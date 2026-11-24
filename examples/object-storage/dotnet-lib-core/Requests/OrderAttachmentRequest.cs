using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TeaQL.Core;
using TeaQL.Runtime;
using Generated.Models;

namespace Generated.Requests
{
    public class OrderAttachmentRequest
    {
        private SelectQuery _query;
        private string? _purpose;
        private string? _comment;
        private bool _countRequested;
        private readonly List<DynamicSearchWarning> _dynamicSearchWarnings = new();
        private static object TeaqlQueryValue(object value) =>
            value is TimeSpan time
                ? time.ToString(@"hh\:mm\:ss", System.Globalization.CultureInfo.InvariantCulture)
                : value;

        public OrderAttachmentRequest()
        {
            _query = new SelectQuery("OrderAttachment");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

        public OrderAttachmentRequest FilterByJson(string source, Action<DynamicSearchWarning>? warn = null)
        {
            var result = DynamicSearch.MergeGeneratedScalar(_query, source, GeneratedSearchModels.Models, warn);
            _query = result.Query;
            _dynamicSearchWarnings.AddRange(result.Warnings);
            return this;
        }

        public IReadOnlyList<DynamicSearchWarning> DynamicSearchWarnings => _dynamicSearchWarnings.ToArray();

        public OrderAttachmentRequest WithDeletedRows()
        {
            _query.Filters.RemoveAll(filter => filter.Field == "version" && filter.Operator == "gt");
            return this;
        }

        public OrderAttachmentRequest DeletedRowsOnly()
        {
            WithDeletedRows();
            _query.AndFilter(new FilterExpression { Operator = "lt", Field = "version", Expected = 0L });
            return this;
        }

        public OrderAttachmentRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutableOrderAttachmentRequest Purpose(string p)
        {
            p = QueryIntent.RequirePurpose(p);
            _query.Purpose(p);
            _purpose = p;
            return new ExecutableOrderAttachmentRequest(
                ExecuteForListInternalAsync,
                ExecuteForOneInternalAsync,
                ExecuteForListWithMetadataInternalAsync,
                ExecuteForStreamInternalAsync,
                c => Comment(c),
                EnsureIntent);
        }

        public OrderAttachmentRequest OptimizeForContinuousPageFetch()
        {
            _query.OptimizeForContinuousPageFetch();
            return this;
        }

        public OrderAttachmentRequest OptimizeForContinuousPageFetchWith(string namespaceName, int ttlSeconds)
        {
            _query.OptimizeForContinuousPageFetchWith(namespaceName, ttlSeconds);
            return this;
        }

        public OrderAttachmentRequest OptimizePaginationWithIdSet()
        {
            _query.OptimizePaginationWithIdSet();
            return this;
        }

        public OrderAttachmentRequest OptimizePaginationWithIdSet(string namespaceName, int ttlSeconds, int maxIds)
        {
            _query.OptimizePaginationWithIdSet(namespaceName, ttlSeconds, maxIds);
            return this;
        }

        public OrderAttachmentRequest TopNProbeParentThreshold(int threshold)
        {
            _query.TopNProbeParentThreshold(threshold);
            return this;
        }

        public OrderAttachmentRequest Limit(int n)
        {
            _query.Limit(n);
            return this;
        }

        public OrderAttachmentRequest Offset(int n)
        {
            _query.Offset(n);
            return this;
        }

        public OrderAttachmentRequest SelectSelfFields()
        {
            _query.Project(global::Generated.Models.OrderAttachment.__TeaqlFields.ID_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.OrderAttachment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.OrderAttachment.__TeaqlFields.FILE_NAME_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_TYPE_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_LENGTH_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_SHA256_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.OrderAttachment.__TeaqlFields.OBJECT_KEY_PROPERTY_QUERY);
            _query.Project(global::Generated.Models.OrderAttachment.__TeaqlFields.VERSION_PROPERTY_QUERY);
            return this;
        }

                public OrderAttachmentRequest SelectId()
                {
                    _query.Project(global::Generated.Models.OrderAttachment.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }


                public OrderAttachmentRequest SelectFileName()
                {
                    _query.Project(global::Generated.Models.OrderAttachment.__TeaqlFields.FILE_NAME_PROPERTY_QUERY);
                    return this;
                }

                public OrderAttachmentRequest SelectContentType()
                {
                    _query.Project(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_TYPE_PROPERTY_QUERY);
                    return this;
                }

                public OrderAttachmentRequest SelectContentLength()
                {
                    _query.Project(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_LENGTH_PROPERTY_QUERY);
                    return this;
                }

                public OrderAttachmentRequest SelectContentSha256()
                {
                    _query.Project(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_SHA256_PROPERTY_QUERY);
                    return this;
                }

                public OrderAttachmentRequest SelectObjectKey()
                {
                    _query.Project(global::Generated.Models.OrderAttachment.__TeaqlFields.OBJECT_KEY_PROPERTY_QUERY);
                    return this;
                }

                public OrderAttachmentRequest SelectVersion()
                {
                    _query.Project(global::Generated.Models.OrderAttachment.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }

                public OrderAttachmentRequest SelectCustomerOrder()
                {
                    return SelectCustomerOrderWith(new CustomerOrderRequest());
                }

                public OrderAttachmentRequest SelectCustomerOrderWith(CustomerOrderRequest related)
                {
                    _query.Project(global::Generated.Models.OrderAttachment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY);
                    _query.ForwardRelationQuery(global::Generated.Models.OrderAttachment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_MEMBER, "CustomerOrder", global::Generated.Models.OrderAttachment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY, related.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectCustomerOrderWith", Relation: global::Generated.Models.OrderAttachment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_MEMBER)
                    };
                    return this;
                }
                public OrderAttachmentRequest WithCustomerOrderMatching(CustomerOrderRequest related)
                {
                    _query.AndFilter(Expr.InSubquery(global::Generated.Models.OrderAttachment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("CustomerOrder")!, related.GetQuery(), "id"));
                    return this;
                }

                public OrderAttachmentRequest WithoutCustomerOrderMatching(CustomerOrderRequest related)
                {
                    _query.AndFilter(Expr.NotInSubquery(global::Generated.Models.OrderAttachment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY, GeneratedRuntimeModule.Module.Metadata.GetEntity("CustomerOrder")!, related.GetQuery(), "id"));
                    return this;
                }

                public OrderAttachmentRequest WithIdIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.OrderAttachment.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderAttachmentRequest WithIdIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.OrderAttachment.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderAttachmentRequest WithIdIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.OrderAttachment.__TeaqlFields.ID_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderAttachmentRequest WithIdNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.OrderAttachment.__TeaqlFields.ID_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderAttachmentRequest WithIdGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.OrderAttachment.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderAttachmentRequest WithIdGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.OrderAttachment.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderAttachmentRequest WithIdLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.OrderAttachment.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderAttachmentRequest WithIdLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.OrderAttachment.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderAttachmentRequest WithIdBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.OrderAttachment.__TeaqlFields.ID_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public OrderAttachmentRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.OrderAttachment.__TeaqlFields.ID_PROPERTY_QUERY));
                    return this;
                }

                public OrderAttachmentRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.OrderAttachment.__TeaqlFields.ID_PROPERTY_QUERY));
                    return this;
                }

                public OrderAttachmentRequest FilterByCustomerOrder(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.OrderAttachment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderAttachmentRequest FilterByCustomerOrderIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.OrderAttachment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY, vals));
                    return this;
                }

                public OrderAttachmentRequest WithCustomerOrderIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.OrderAttachment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY));
                    return this;
                }

                public OrderAttachmentRequest WithCustomerOrderIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.OrderAttachment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY));
                    return this;
                }

                public OrderAttachmentRequest WithFileNameContaining(string val)
                {
                    _query.AndFilter(Expr.Contain(global::Generated.Models.OrderAttachment.__TeaqlFields.FILE_NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderAttachmentRequest WithFileNameIs(string val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.OrderAttachment.__TeaqlFields.FILE_NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }
                public OrderAttachmentRequest WithFileNameIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.OrderAttachment.__TeaqlFields.FILE_NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderAttachmentRequest WithFileNameIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.OrderAttachment.__TeaqlFields.FILE_NAME_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderAttachmentRequest WithFileNameNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.OrderAttachment.__TeaqlFields.FILE_NAME_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderAttachmentRequest WithFileNameGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.OrderAttachment.__TeaqlFields.FILE_NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderAttachmentRequest WithFileNameGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.OrderAttachment.__TeaqlFields.FILE_NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderAttachmentRequest WithFileNameLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.OrderAttachment.__TeaqlFields.FILE_NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderAttachmentRequest WithFileNameLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.OrderAttachment.__TeaqlFields.FILE_NAME_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderAttachmentRequest WithFileNameBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.OrderAttachment.__TeaqlFields.FILE_NAME_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public OrderAttachmentRequest WithFileNameIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.OrderAttachment.__TeaqlFields.FILE_NAME_PROPERTY_QUERY));
                    return this;
                }

                public OrderAttachmentRequest WithFileNameIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.OrderAttachment.__TeaqlFields.FILE_NAME_PROPERTY_QUERY));
                    return this;
                }
                public OrderAttachmentRequest WithFileNameNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain(global::Generated.Models.OrderAttachment.__TeaqlFields.FILE_NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderAttachmentRequest WithFileNameStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith(global::Generated.Models.OrderAttachment.__TeaqlFields.FILE_NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderAttachmentRequest WithFileNameNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith(global::Generated.Models.OrderAttachment.__TeaqlFields.FILE_NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderAttachmentRequest WithFileNameEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith(global::Generated.Models.OrderAttachment.__TeaqlFields.FILE_NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderAttachmentRequest WithFileNameNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith(global::Generated.Models.OrderAttachment.__TeaqlFields.FILE_NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderAttachmentRequest WithFileNameSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike(global::Generated.Models.OrderAttachment.__TeaqlFields.FILE_NAME_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderAttachmentRequest WithContentTypeContaining(string val)
                {
                    _query.AndFilter(Expr.Contain(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_TYPE_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderAttachmentRequest WithContentTypeIs(string val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_TYPE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }
                public OrderAttachmentRequest WithContentTypeIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_TYPE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderAttachmentRequest WithContentTypeIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_TYPE_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderAttachmentRequest WithContentTypeNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_TYPE_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderAttachmentRequest WithContentTypeGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_TYPE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderAttachmentRequest WithContentTypeGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_TYPE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderAttachmentRequest WithContentTypeLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_TYPE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderAttachmentRequest WithContentTypeLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_TYPE_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderAttachmentRequest WithContentTypeBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_TYPE_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public OrderAttachmentRequest WithContentTypeIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_TYPE_PROPERTY_QUERY));
                    return this;
                }

                public OrderAttachmentRequest WithContentTypeIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_TYPE_PROPERTY_QUERY));
                    return this;
                }
                public OrderAttachmentRequest WithContentTypeNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_TYPE_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderAttachmentRequest WithContentTypeStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_TYPE_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderAttachmentRequest WithContentTypeNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_TYPE_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderAttachmentRequest WithContentTypeEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_TYPE_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderAttachmentRequest WithContentTypeNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_TYPE_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderAttachmentRequest WithContentTypeSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_TYPE_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderAttachmentRequest WithContentLengthIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_LENGTH_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderAttachmentRequest WithContentLengthIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_LENGTH_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderAttachmentRequest WithContentLengthIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_LENGTH_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderAttachmentRequest WithContentLengthNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_LENGTH_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderAttachmentRequest WithContentLengthGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_LENGTH_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderAttachmentRequest WithContentLengthGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_LENGTH_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderAttachmentRequest WithContentLengthLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_LENGTH_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderAttachmentRequest WithContentLengthLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_LENGTH_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderAttachmentRequest WithContentLengthBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_LENGTH_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public OrderAttachmentRequest WithContentLengthIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_LENGTH_PROPERTY_QUERY));
                    return this;
                }

                public OrderAttachmentRequest WithContentLengthIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_LENGTH_PROPERTY_QUERY));
                    return this;
                }

                public OrderAttachmentRequest WithContentSha256Containing(string val)
                {
                    _query.AndFilter(Expr.Contain(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_SHA256_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderAttachmentRequest WithContentSha256Is(string val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_SHA256_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }
                public OrderAttachmentRequest WithContentSha256IsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_SHA256_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderAttachmentRequest WithContentSha256In(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_SHA256_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderAttachmentRequest WithContentSha256NotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_SHA256_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderAttachmentRequest WithContentSha256GreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_SHA256_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderAttachmentRequest WithContentSha256GreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_SHA256_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderAttachmentRequest WithContentSha256LessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_SHA256_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderAttachmentRequest WithContentSha256LessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_SHA256_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderAttachmentRequest WithContentSha256Between(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_SHA256_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public OrderAttachmentRequest WithContentSha256IsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_SHA256_PROPERTY_QUERY));
                    return this;
                }

                public OrderAttachmentRequest WithContentSha256IsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_SHA256_PROPERTY_QUERY));
                    return this;
                }
                public OrderAttachmentRequest WithContentSha256NotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_SHA256_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderAttachmentRequest WithContentSha256StartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_SHA256_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderAttachmentRequest WithContentSha256NotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_SHA256_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderAttachmentRequest WithContentSha256EndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_SHA256_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderAttachmentRequest WithContentSha256NotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_SHA256_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderAttachmentRequest WithContentSha256SoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_SHA256_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderAttachmentRequest WithObjectKeyContaining(string val)
                {
                    _query.AndFilter(Expr.Contain(global::Generated.Models.OrderAttachment.__TeaqlFields.OBJECT_KEY_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderAttachmentRequest WithObjectKeyIs(string val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.OrderAttachment.__TeaqlFields.OBJECT_KEY_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }
                public OrderAttachmentRequest WithObjectKeyIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.OrderAttachment.__TeaqlFields.OBJECT_KEY_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderAttachmentRequest WithObjectKeyIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.OrderAttachment.__TeaqlFields.OBJECT_KEY_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderAttachmentRequest WithObjectKeyNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.OrderAttachment.__TeaqlFields.OBJECT_KEY_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderAttachmentRequest WithObjectKeyGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.OrderAttachment.__TeaqlFields.OBJECT_KEY_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderAttachmentRequest WithObjectKeyGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.OrderAttachment.__TeaqlFields.OBJECT_KEY_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderAttachmentRequest WithObjectKeyLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.OrderAttachment.__TeaqlFields.OBJECT_KEY_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderAttachmentRequest WithObjectKeyLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.OrderAttachment.__TeaqlFields.OBJECT_KEY_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderAttachmentRequest WithObjectKeyBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.OrderAttachment.__TeaqlFields.OBJECT_KEY_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public OrderAttachmentRequest WithObjectKeyIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.OrderAttachment.__TeaqlFields.OBJECT_KEY_PROPERTY_QUERY));
                    return this;
                }

                public OrderAttachmentRequest WithObjectKeyIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.OrderAttachment.__TeaqlFields.OBJECT_KEY_PROPERTY_QUERY));
                    return this;
                }
                public OrderAttachmentRequest WithObjectKeyNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain(global::Generated.Models.OrderAttachment.__TeaqlFields.OBJECT_KEY_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderAttachmentRequest WithObjectKeyStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith(global::Generated.Models.OrderAttachment.__TeaqlFields.OBJECT_KEY_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderAttachmentRequest WithObjectKeyNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith(global::Generated.Models.OrderAttachment.__TeaqlFields.OBJECT_KEY_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderAttachmentRequest WithObjectKeyEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith(global::Generated.Models.OrderAttachment.__TeaqlFields.OBJECT_KEY_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderAttachmentRequest WithObjectKeyNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith(global::Generated.Models.OrderAttachment.__TeaqlFields.OBJECT_KEY_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderAttachmentRequest WithObjectKeySoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike(global::Generated.Models.OrderAttachment.__TeaqlFields.OBJECT_KEY_PROPERTY_QUERY, val));
                    return this;
                }

                public OrderAttachmentRequest WithVersionIs(object val)
                {
                    _query.AndFilter(Expr.Eq(global::Generated.Models.OrderAttachment.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderAttachmentRequest WithVersionIsNot(object val)
                {
                    _query.AndFilter(Expr.Ne(global::Generated.Models.OrderAttachment.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderAttachmentRequest WithVersionIn(params object[] vals)
                {
                    _query.AndFilter(Expr.In(global::Generated.Models.OrderAttachment.__TeaqlFields.VERSION_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderAttachmentRequest WithVersionNotIn(params object[] vals)
                {
                    _query.AndFilter(Expr.NotIn(global::Generated.Models.OrderAttachment.__TeaqlFields.VERSION_PROPERTY_QUERY, vals.Select(TeaqlQueryValue).ToArray()));
                    return this;
                }

                public OrderAttachmentRequest WithVersionGreaterThan(object val)
                {
                    _query.AndFilter(Expr.Gt(global::Generated.Models.OrderAttachment.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderAttachmentRequest WithVersionGreaterThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Gte(global::Generated.Models.OrderAttachment.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderAttachmentRequest WithVersionLessThan(object val)
                {
                    _query.AndFilter(Expr.Lt(global::Generated.Models.OrderAttachment.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderAttachmentRequest WithVersionLessThanOrEqualTo(object val)
                {
                    _query.AndFilter(Expr.Lte(global::Generated.Models.OrderAttachment.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(val)));
                    return this;
                }

                public OrderAttachmentRequest WithVersionBetween(object lower, object upper)
                {
                    _query.AndFilter(Expr.Between(global::Generated.Models.OrderAttachment.__TeaqlFields.VERSION_PROPERTY_QUERY, TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public OrderAttachmentRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull(global::Generated.Models.OrderAttachment.__TeaqlFields.VERSION_PROPERTY_QUERY));
                    return this;
                }

                public OrderAttachmentRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull(global::Generated.Models.OrderAttachment.__TeaqlFields.VERSION_PROPERTY_QUERY));
                    return this;
                }

                public OrderAttachmentRequest OrderByIdAscending()
                {
                    _query.OrderBy(global::Generated.Models.OrderAttachment.__TeaqlFields.ID_PROPERTY_QUERY, "asc");
                    return this;
                }

                public OrderAttachmentRequest OrderByIdDescending()
                {
                    _query.OrderBy(global::Generated.Models.OrderAttachment.__TeaqlFields.ID_PROPERTY_QUERY, "desc");
                    return this;
                }


                public OrderAttachmentRequest OrderByFileNameAscending()
                {
                    _query.OrderBy(global::Generated.Models.OrderAttachment.__TeaqlFields.FILE_NAME_PROPERTY_QUERY, "asc");
                    return this;
                }

                public OrderAttachmentRequest OrderByFileNameDescending()
                {
                    _query.OrderBy(global::Generated.Models.OrderAttachment.__TeaqlFields.FILE_NAME_PROPERTY_QUERY, "desc");
                    return this;
                }

                public OrderAttachmentRequest OrderByContentTypeAscending()
                {
                    _query.OrderBy(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_TYPE_PROPERTY_QUERY, "asc");
                    return this;
                }

                public OrderAttachmentRequest OrderByContentTypeDescending()
                {
                    _query.OrderBy(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_TYPE_PROPERTY_QUERY, "desc");
                    return this;
                }

                public OrderAttachmentRequest OrderByContentLengthAscending()
                {
                    _query.OrderBy(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_LENGTH_PROPERTY_QUERY, "asc");
                    return this;
                }

                public OrderAttachmentRequest OrderByContentLengthDescending()
                {
                    _query.OrderBy(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_LENGTH_PROPERTY_QUERY, "desc");
                    return this;
                }

                public OrderAttachmentRequest OrderByContentSha256Ascending()
                {
                    _query.OrderBy(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_SHA256_PROPERTY_QUERY, "asc");
                    return this;
                }

                public OrderAttachmentRequest OrderByContentSha256Descending()
                {
                    _query.OrderBy(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_SHA256_PROPERTY_QUERY, "desc");
                    return this;
                }

                public OrderAttachmentRequest OrderByObjectKeyAscending()
                {
                    _query.OrderBy(global::Generated.Models.OrderAttachment.__TeaqlFields.OBJECT_KEY_PROPERTY_QUERY, "asc");
                    return this;
                }

                public OrderAttachmentRequest OrderByObjectKeyDescending()
                {
                    _query.OrderBy(global::Generated.Models.OrderAttachment.__TeaqlFields.OBJECT_KEY_PROPERTY_QUERY, "desc");
                    return this;
                }

                public OrderAttachmentRequest OrderByVersionAscending()
                {
                    _query.OrderBy(global::Generated.Models.OrderAttachment.__TeaqlFields.VERSION_PROPERTY_QUERY, "asc");
                    return this;
                }

                public OrderAttachmentRequest OrderByVersionDescending()
                {
                    _query.OrderBy(global::Generated.Models.OrderAttachment.__TeaqlFields.VERSION_PROPERTY_QUERY, "desc");
                    return this;
                }


        public OrderAttachmentRequest Count()
        {
            return CountAs("count");
        }

        public OrderAttachmentRequest CountAs(string retName)
        {
            _countRequested = true;
            _query.Aggregate("Count", "id", retName);
            _query.PageCountOperation = new QueryOperation("count", "CountAs", retName);
            return this;
        }

                public OrderAttachmentRequest MinContentLength()
                {
                    return MinContentLengthAs("minOfContentLength");
                }

                public OrderAttachmentRequest MinContentLengthAs(string retName)
                {
                    _query.Aggregate("min", global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_LENGTH_PROPERTY_QUERY, retName);
                    return this;
                }
                public OrderAttachmentRequest MaxContentLength()
                {
                    return MaxContentLengthAs("maxOfContentLength");
                }

                public OrderAttachmentRequest MaxContentLengthAs(string retName)
                {
                    _query.Aggregate("max", global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_LENGTH_PROPERTY_QUERY, retName);
                    return this;
                }
                public OrderAttachmentRequest SumContentLength()
                {
                    return SumContentLengthAs("sumOfContentLength");
                }

                public OrderAttachmentRequest SumContentLengthAs(string retName)
                {
                    _query.Aggregate("sum", global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_LENGTH_PROPERTY_QUERY, retName);
                    return this;
                }
                public OrderAttachmentRequest AvgContentLength()
                {
                    return AvgContentLengthAs("avgOfContentLength");
                }

                public OrderAttachmentRequest AvgContentLengthAs(string retName)
                {
                    _query.Aggregate("avg", global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_LENGTH_PROPERTY_QUERY, retName);
                    return this;
                }
                public OrderAttachmentRequest StandardDeviationContentLength()
                {
                    return StandardDeviationContentLengthAs("standardDeviationOfContentLength");
                }

                public OrderAttachmentRequest StandardDeviationContentLengthAs(string retName)
                {
                    _query.Aggregate("stddev", global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_LENGTH_PROPERTY_QUERY, retName);
                    return this;
                }
                public OrderAttachmentRequest SquareRootOfPopulationStandardDeviationContentLength()
                {
                    return SquareRootOfPopulationStandardDeviationContentLengthAs("squareRootOfPopulationStandardDeviationOfContentLength");
                }

                public OrderAttachmentRequest SquareRootOfPopulationStandardDeviationContentLengthAs(string retName)
                {
                    _query.Aggregate("stddev_pop", global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_LENGTH_PROPERTY_QUERY, retName);
                    return this;
                }
                public OrderAttachmentRequest SampleVarianceContentLength()
                {
                    return SampleVarianceContentLengthAs("sampleVarianceOfContentLength");
                }

                public OrderAttachmentRequest SampleVarianceContentLengthAs(string retName)
                {
                    _query.Aggregate("var_samp", global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_LENGTH_PROPERTY_QUERY, retName);
                    return this;
                }
                public OrderAttachmentRequest SamplePopulationVarianceContentLength()
                {
                    return SamplePopulationVarianceContentLengthAs("samplePopulationVarianceOfContentLength");
                }

                public OrderAttachmentRequest SamplePopulationVarianceContentLengthAs(string retName)
                {
                    _query.Aggregate("var_pop", global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_LENGTH_PROPERTY_QUERY, retName);
                    return this;
                }
                public OrderAttachmentRequest GroupById()
                {
                    _query.GroupBy(global::Generated.Models.OrderAttachment.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }

                public OrderAttachmentRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.OrderAttachment.__TeaqlFields.ID_PROPERTY_QUERY);
                    return this;
                }
                public OrderAttachmentRequest GroupByCustomerOrder()
                {
                    _query.GroupBy(global::Generated.Models.OrderAttachment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY);
                    return this;
                }

                public OrderAttachmentRequest GroupByCustomerOrderAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.OrderAttachment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY);
                    return this;
                }
                public OrderAttachmentRequest GroupByFileName()
                {
                    _query.GroupBy(global::Generated.Models.OrderAttachment.__TeaqlFields.FILE_NAME_PROPERTY_QUERY);
                    return this;
                }

                public OrderAttachmentRequest GroupByFileNameAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.OrderAttachment.__TeaqlFields.FILE_NAME_PROPERTY_QUERY);
                    return this;
                }
                public OrderAttachmentRequest GroupByContentType()
                {
                    _query.GroupBy(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_TYPE_PROPERTY_QUERY);
                    return this;
                }

                public OrderAttachmentRequest GroupByContentTypeAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_TYPE_PROPERTY_QUERY);
                    return this;
                }
                public OrderAttachmentRequest GroupByContentLength()
                {
                    _query.GroupBy(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_LENGTH_PROPERTY_QUERY);
                    return this;
                }

                public OrderAttachmentRequest GroupByContentLengthAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_LENGTH_PROPERTY_QUERY);
                    return this;
                }
                public OrderAttachmentRequest GroupByContentSha256()
                {
                    _query.GroupBy(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_SHA256_PROPERTY_QUERY);
                    return this;
                }

                public OrderAttachmentRequest GroupByContentSha256As(string retName)
                {
                    _query.GroupBy(global::Generated.Models.OrderAttachment.__TeaqlFields.CONTENT_SHA256_PROPERTY_QUERY);
                    return this;
                }
                public OrderAttachmentRequest GroupByObjectKey()
                {
                    _query.GroupBy(global::Generated.Models.OrderAttachment.__TeaqlFields.OBJECT_KEY_PROPERTY_QUERY);
                    return this;
                }

                public OrderAttachmentRequest GroupByObjectKeyAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.OrderAttachment.__TeaqlFields.OBJECT_KEY_PROPERTY_QUERY);
                    return this;
                }
                public OrderAttachmentRequest GroupByVersion()
                {
                    _query.GroupBy(global::Generated.Models.OrderAttachment.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }

                public OrderAttachmentRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy(global::Generated.Models.OrderAttachment.__TeaqlFields.VERSION_PROPERTY_QUERY);
                    return this;
                }
                public OrderAttachmentRequest FacetByCustomerOrderAs(
                    string name, CustomerOrderRequest request,
                    bool includeAllFacets = true)
                {
                    _query.Facets.Add(new FacetRequest(
                        name, global::Generated.Models.OrderAttachment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_MEMBER, request.GetQuery(), includeAllFacets) {
                        Operation = new QueryOperation("facet", "FacetByCustomerOrderAs", name, global::Generated.Models.OrderAttachment.__TeaqlFields.CUSTOMER_ORDER_PROPERTY_QUERY)
                    });
                    return this;
                }


        private async Task<QueryResult> ExecuteForListInternalAsync(UserContext context)
        {
            EnsureIntent();
            var service = context.RequireResource<IDataService>();
            var req = context.PrepareQueryRequest(new QueryRequest(_query, new QueryIntent(_comment, _purpose)));
            // The runtime owns authorized membership, recursive Facets and SQL provenance.
            return await service.QueryAsync(req);
        }

        private async Task<Generated.Models.OrderAttachment?> ExecuteForOneInternalAsync(UserContext context)
        {
            var entities = await ExecuteTypedListInternalAsync(context, true);
            return entities.Count == 0 ? null : entities[0];
        }

        private Task<SmartList<Generated.Models.OrderAttachment>> ExecuteForListWithMetadataInternalAsync(UserContext context)
            => ExecuteTypedListInternalAsync(context, false);

        private async Task<SmartList<Generated.Models.OrderAttachment>> ExecuteTypedListInternalAsync(UserContext context, bool limitOne)
        {
            EnsureIntent();
            var service = context.RequireResource<IDataService>();
            var query = _query.CloneForExecution();
            if (limitOne) query.Limit(1);
            var request = context.PrepareQueryRequest(new QueryRequest(query, new QueryIntent(_comment, _purpose)));
            if (request.Query.GroupByItems.Count > 0 || request.Query.ObjectGroupBys.Count > 0)
                throw new InvalidOperationException("Grouped projections require ExecuteForRowsAsync");
            SmartList<Record> result;
            if (_countRequested) result = await RuntimeDataService.FetchPageAsync(service, context, request);
            else {
                var rows = await service.QueryAsync(request);
                result = new SmartList<Record>(rows.Rows) { Facets = rows.Facets };
            }
            var alias = _countRequested ? request.Query.PageCountOperation?.Alias : null;
            var aggregateOnly = request.Query.AggregateItems.Any(item =>
                !(alias != null && item.Function == AggregateFunction.Count && item.Alias == alias));
            if (aggregateOnly) {
                if (result.Count > 0) foreach (var pair in result[0]) result.Aggregations[pair.Key] = pair.Value;
                result.Clear();
            }
            var entities = new SmartList<Generated.Models.OrderAttachment>() {
                TotalCount = result.TotalCount, Aggregations = result.Aggregations,
                Summary = result.Summary, Facets = result.Facets
            };
            foreach (var row in result)
                entities.Add(Generated.Models.OrderAttachment.FromRecord(row, new EntityRoot()));
            global::TeaQL.Runtime.EntityQueryProperties.CaptureList(entities, request.Query);
            return entities;
        }

        private IAsyncEnumerable<Generated.Models.OrderAttachment> ExecuteForStreamInternalAsync(
            UserContext context,
            int chunkSize,
            CancellationToken cancellationToken = default)
        {
            EnsureIntent();
            var service = context.RequireResource<IDataService>();
            if (service is not IStreamQueryExecutor streaming)
                throw new NotSupportedException("The configured data service does not provide a local streaming cursor; federation streaming requires a separate protocol");
            var request = context.PrepareQueryRequest(new QueryRequest(_query, new QueryIntent(_comment, _purpose)));
            if (request.Query.AggregateItems.Count > 0 || request.Query.GroupByItems.Count > 0 || request.Query.ObjectGroupBys.Count > 0)
                throw new InvalidOperationException("Entity streams require entity rows; use a record or aggregation query");
            return RuntimeDataService.StreamEntities(context, request,
                row => global::TeaQL.Runtime.EntityQueryProperties.Capture(
                    Generated.Models.OrderAttachment.FromRecord(row, new EntityRoot()), request.Query),
                chunkSize, cancellationToken);
        }

        private void EnsureIntent()
        {
            _ = new QueryIntent(_comment, _purpose);
        }

    }

    public sealed class ExecutableOrderAttachmentRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, Task<Generated.Models.OrderAttachment?>> _executeForOne;
        private readonly Func<UserContext, Task<SmartList<Generated.Models.OrderAttachment>>> _executeForList;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.OrderAttachment>> _executeForStream;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutableOrderAttachmentRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, Task<Generated.Models.OrderAttachment?>> executeForOne,
            Func<UserContext, Task<SmartList<Generated.Models.OrderAttachment>>> executeForList,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.OrderAttachment>> executeForStream,
            Action<string> comment,
            Action ensureIntent)
        {
            _executeForRows = executeForRows;
            _executeForOne = executeForOne;
            _executeForList = executeForList;
            _executeForStream = executeForStream;
            _comment = comment;
            _ensureIntent = ensureIntent;
        }

        public ExecutableOrderAttachmentRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public async Task<Generated.Models.OrderAttachment> NewEntityAsync(UserContext context)
        {
            _ensureIntent();
            ArgumentNullException.ThrowIfNull(context);
            var identity = checked((long)await context.NextIdAsync("OrderAttachment").ConfigureAwait(false));
            return context.InitializeEntity("OrderAttachment", Generated.Models.OrderAttachment.TeaqlNewWithReservedId(identity),
                entity => entity.TeaqlValidateReservedIdentity(identity));
        }
        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public Task<SmartList<Generated.Models.OrderAttachment>> ExecuteForListAsync(UserContext context)
            => _executeForList(context);

        public IAsyncEnumerable<Generated.Models.OrderAttachment> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public Task<Generated.Models.OrderAttachment?> ExecuteForOneAsync(
            UserContext context)
            => _executeForOne(context);
    }

}