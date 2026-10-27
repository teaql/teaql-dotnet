using TeaQL.Core;
using TeaQL.DataService;
using TeaQL.Runtime;

namespace TeaQL.BlobReference;

/// <summary>Uses only the SQL owner's row images and physical lease for modeled bindings.</summary>
internal sealed class SqliteBlobMutationParticipant(SqliteBlobCatalog catalog,
    IBlobBindingAuthorization authorization, UserContext actor) : IBlobMutationParticipant
{
    private static InvalidOperationException Error(string code) => new(code);

    public async Task ApplyAsync(IPhysicalSqlOwner owner, MutationRequest request,
        EntityDescriptor entity, Record? previous, Record? persisted)
    {
        var idProperty = entity.IdProperty() ?? throw Error("BLOB_REFERENCE_INVALID");
        var idValue = request switch
        {
            InsertMutationRequest insert => insert.Command.Values.GetValueOrDefault(idProperty.Name),
            UpdateMutationRequest update => update.Command.Id,
            DeleteMutationRequest delete => delete.Command.Id,
            _ => null
        };
        var id = idValue?.TryU64();
        if (id is null or 0) throw Error("BLOB_REFERENCE_INVALID");
        string scope;
        try { scope = authorization.Scope(actor); }
        catch { throw Error("BLOB_BIND_DENIED"); }
        if (string.IsNullOrWhiteSpace(scope) || scope != scope.Trim() ||
            System.Text.Encoding.UTF8.GetByteCount(scope) > 256 || scope.Any(char.IsControl))
            throw Error("BLOB_BIND_DENIED");

        foreach (var field in entity.Properties.Where(p => p.DataType == DataType.BlobRef || p.BlobSpec != null))
        {
            if (field.DataType != DataType.BlobRef || field.BlobSpec is not { } spec ||
                spec.Entity != entity.Name || spec.Field != field.Name || spec.Nullable != field.Nullable)
                throw Error("BLOB_CAPABILITY_MISSING");
            var before = previous is null ? null : Reference(previous, field);
            var after = request is DeleteMutationRequest ? null :
                persisted is null ? throw Error("BLOB_REFERENCE_INVALID") : Reference(persisted, field);
            var target = new BlobBindingTarget(entity.Name, id.Value.ToString(), field.Name);
            var binding = await owner.WithPhysicalSqlAsync(async lease =>
            {
                var rows = await lease.FetchAllAsync("""
                    SELECT b.key,b.version,c.status FROM teaql_blob_binding b
                    LEFT JOIN teaql_blob_catalog c ON c.key=b.key AND c.scope=b.scope
                    WHERE b.scope=@p0 AND b.entity=@p1 AND b.entity_id=@p2 AND b.field=@p3
                    """, [new Value.TextValue(scope), new Value.TextValue(target.Entity),
                        new Value.TextValue(target.EntityId), new Value.TextValue(target.Field)]);
                if (rows.Count > 1) throw Error("BLOB_CATALOG_FAILED");
                return rows.SingleOrDefault();
            });
            long version;
            if (previous == null)
            {
                if (request is not InsertMutationRequest || binding != null)
                    throw Error("BLOB_BIND_DENIED");
                version = 0;
            }
            else
            {
                version = binding?.GetValueOrDefault("version")?.TryI64() ?? 0;
                if (request is InsertMutationRequest || binding == null || version <= 0 ||
                    !Equals(CatalogReference(binding), before) ||
                    before != null && binding.GetValueOrDefault("status")?.TryText() != "available")
                    throw Error("BLOB_BIND_DENIED");
            }
            var action = request switch
            {
                InsertMutationRequest => "Create",
                DeleteMutationRequest => "Delete",
                _ => "Update"
            };
            if (previous != null && Equals(before, after) && request is not DeleteMutationRequest)
            {
                bool allowed;
                try { allowed = await authorization.AuthorizeAsync(actor, target, before, after, action); }
                catch { throw Error("BLOB_BIND_DENIED"); }
                if (!allowed) throw Error("BLOB_BIND_DENIED");
                continue;
            }
            if (version == long.MaxValue) throw Error("BLOB_BIND_DENIED");
            await catalog.BindInOwnerAsync(owner, actor,
                new BlobBindingChange(target, after, version, version + 1, action), authorization);
        }
    }

    private static BlobRef? Reference(Record row, PropertyDescriptor field)
    {
        if (!row.TryGetValue(field.Name, out var value) || value is Value.NullValue or Value.TypedNullValue)
        {
            if (field.Nullable) return null;
            throw Error("BLOB_REFERENCE_INVALID");
        }
        try
        {
            return value switch
            {
                Value.BlobRefValue(var reference) => reference,
                Value.TextValue(var text) => new BlobRef(text),
                _ => throw Error("BLOB_REFERENCE_INVALID")
            };
        }
        catch (ArgumentException) { throw Error("BLOB_REFERENCE_INVALID"); }
    }

    private static BlobRef? CatalogReference(Record row)
    {
        var value = row.GetValueOrDefault("key");
        if (value is null or Value.NullValue) return null;
        try { return new BlobRef(value.TryText() ?? throw Error("BLOB_CATALOG_FAILED")); }
        catch (ArgumentException) { throw Error("BLOB_CATALOG_FAILED"); }
    }
}
