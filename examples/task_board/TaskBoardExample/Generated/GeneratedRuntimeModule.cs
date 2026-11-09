using TeaQL.Core;

namespace Generated;

internal sealed class PlatformChecker : IEntityChecker
{
    public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
    {
        var values = request is InsertMutationRequest insert ? insert.Command.Values
            : request is UpdateMutationRequest update ? update.Command.Values : new Record();
        var creating = request is InsertMutationRequest;
        var updating = request is UpdateMutationRequest;
        var results = new List<CheckResult>();
        if (creating && (!values.TryGetValue("founded", out var createFounded) || createFounded is Value.NullValue)) {
            values["founded"] = new Value.TimestampValue(now.ToUnixTimeMilliseconds());
            context.RecordFixEvidence(new FixEvidence("Platform", "founded", FixEvidenceSource.Clock, "graphClock"));
        }



        if ((creating && !values.ContainsKey("name")) || (values.TryGetValue("name", out var checkName) && checkName is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "Platform", Location = ObjectLocation.Property("name") });
        if (values.TryGetValue("name", out var maxLenName) && maxLenName.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", EntityType = "Platform", Location = ObjectLocation.Property("name"), SystemValue = 100 });

        if ((creating && !values.ContainsKey("founded")) || (values.TryGetValue("founded", out var checkFounded) && checkFounded is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "Platform", Location = ObjectLocation.Property("founded") });

        if ((creating && !values.ContainsKey("user_email")) || (values.TryGetValue("user_email", out var checkUserEmail) && checkUserEmail is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "Platform", Location = ObjectLocation.Property("user_email") });
        if (values.TryGetValue("user_email", out var maxLenUserEmail) && maxLenUserEmail.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", EntityType = "Platform", Location = ObjectLocation.Property("user_email"), SystemValue = 100 });


        return results;
    }
}

internal sealed class TaskStatusChecker : IEntityChecker
{
    public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
    {
        var values = request is InsertMutationRequest insert ? insert.Command.Values
            : request is UpdateMutationRequest update ? update.Command.Values : new Record();
        var creating = request is InsertMutationRequest;
        var updating = request is UpdateMutationRequest;
        var results = new List<CheckResult>();
        if ((creating && !values.ContainsKey("name")) || (values.TryGetValue("name", out var checkName) && checkName is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "TaskStatus", Location = ObjectLocation.Property("name") });
        if (values.TryGetValue("name", out var maxLenName) && maxLenName.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", EntityType = "TaskStatus", Location = ObjectLocation.Property("name"), SystemValue = 100 });

        if ((creating && !values.ContainsKey("code")) || (values.TryGetValue("code", out var checkCode) && checkCode is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "TaskStatus", Location = ObjectLocation.Property("code") });
        if (values.TryGetValue("code", out var maxLenCode) && maxLenCode.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", EntityType = "TaskStatus", Location = ObjectLocation.Property("code"), SystemValue = 100 });

        if ((creating && !values.ContainsKey("color")) || (values.TryGetValue("color", out var checkColor) && checkColor is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "TaskStatus", Location = ObjectLocation.Property("color") });
        if (values.TryGetValue("color", out var maxLenColor) && maxLenColor.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", EntityType = "TaskStatus", Location = ObjectLocation.Property("color"), SystemValue = 100 });

        if ((creating && !values.ContainsKey("display_order")) || (values.TryGetValue("display_order", out var checkDisplayOrder) && checkDisplayOrder is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "TaskStatus", Location = ObjectLocation.Property("display_order") });

        if ((creating && !values.ContainsKey("progress")) || (values.TryGetValue("progress", out var checkProgress) && checkProgress is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "TaskStatus", Location = ObjectLocation.Property("progress") });

        if ((creating && !values.ContainsKey("platform")) || (values.TryGetValue("platform", out var checkPlatform) && checkPlatform is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "TaskStatus", Location = ObjectLocation.Property("platform") });


        return results;
    }
}

internal sealed class TaskChecker : IEntityChecker
{
    public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
    {
        var values = request is InsertMutationRequest insert ? insert.Command.Values
            : request is UpdateMutationRequest update ? update.Command.Values : new Record();
        var creating = request is InsertMutationRequest;
        var updating = request is UpdateMutationRequest;
        var results = new List<CheckResult>();
        if ((creating && !values.ContainsKey("name")) || (values.TryGetValue("name", out var checkName) && checkName is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "Task", Location = ObjectLocation.Property("name") });
        if (values.TryGetValue("name", out var minLenName) && minLenName.Raw?.ToString()?.Length.CompareTo(1) == -1) results.Add(new CheckResult { RuleId = "min_length", EntityType = "Task", Location = ObjectLocation.Property("name"), SystemValue = 1 });
        if (values.TryGetValue("name", out var maxLenName) && maxLenName.Raw?.ToString()?.Length > 200) results.Add(new CheckResult { RuleId = "max_length", EntityType = "Task", Location = ObjectLocation.Property("name"), SystemValue = 200 });

        if ((creating && !values.ContainsKey("status")) || (values.TryGetValue("status", out var checkStatus) && checkStatus is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "Task", Location = ObjectLocation.Property("status") });

        if ((creating && !values.ContainsKey("platform")) || (values.TryGetValue("platform", out var checkPlatform) && checkPlatform is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "Task", Location = ObjectLocation.Property("platform") });


        return results;
    }
}

internal sealed class TaskExecutionLogChecker : IEntityChecker
{
    public IReadOnlyList<CheckResult> CheckAndFix(UserContext context, MutationRequest request, DateTimeOffset now)
    {
        var values = request is InsertMutationRequest insert ? insert.Command.Values
            : request is UpdateMutationRequest update ? update.Command.Values : new Record();
        var creating = request is InsertMutationRequest;
        var updating = request is UpdateMutationRequest;
        var results = new List<CheckResult>();
        if ((creating && !values.ContainsKey("task")) || (values.TryGetValue("task", out var checkTask) && checkTask is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "TaskExecutionLog", Location = ObjectLocation.Property("task") });

        if ((creating && !values.ContainsKey("action")) || (values.TryGetValue("action", out var checkAction) && checkAction is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "TaskExecutionLog", Location = ObjectLocation.Property("action") });
        if (values.TryGetValue("action", out var maxLenAction) && maxLenAction.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", EntityType = "TaskExecutionLog", Location = ObjectLocation.Property("action"), SystemValue = 100 });

        if ((creating && !values.ContainsKey("detail")) || (values.TryGetValue("detail", out var checkDetail) && checkDetail is Value.NullValue)) results.Add(new CheckResult { RuleId = "required", EntityType = "TaskExecutionLog", Location = ObjectLocation.Property("detail") });
        if (values.TryGetValue("detail", out var maxLenDetail) && maxLenDetail.Raw?.ToString()?.Length > 100) results.Add(new CheckResult { RuleId = "max_length", EntityType = "TaskExecutionLog", Location = ObjectLocation.Property("detail"), SystemValue = 100 });


        return results;
    }
}

// Passive generated manifest. EnsureSchemaAsync remains an explicit application action.
public static class GeneratedRuntimeModule
{
    public static RuntimeModule Module { get; } = new RuntimeModule(new[]
    {
       "Platform",
       "TaskStatus",
       "Task",
       "TaskExecutionLog"
    }, new Dictionary<string, IEntityChecker>
    {
       ["Platform"] = new PlatformChecker(),
       ["TaskStatus"] = new TaskStatusChecker(),
       ["Task"] = new TaskChecker(),
       ["TaskExecutionLog"] = new TaskExecutionLogChecker()
    }, new Dictionary<string, Record>
    {
       ["Platform"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["name"] = new Value.TextValue(""),
            ["founded"] = new Value.TimestampValue(0),
            ["user_email"] = new Value.TextValue(""),
            ["version"] = new Value.I64Value(0)
        },
       ["TaskStatus"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["name"] = new Value.TextValue(""),
            ["code"] = new Value.TextValue(""),
            ["color"] = new Value.TextValue(""),
            ["display_order"] = new Value.DecimalValue(0),
            ["progress"] = new Value.DecimalValue(0),
            ["platform"] = new Value.I64Value(0),
            ["version"] = new Value.I64Value(0)
        },
       ["Task"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["name"] = new Value.TextValue(""),
            ["status"] = new Value.I64Value(0),
            ["platform"] = new Value.I64Value(0),
            ["version"] = new Value.I64Value(0)
        },
       ["TaskExecutionLog"] = new Record {
            ["id"] = new Value.I64Value(0),
            ["task"] = new Value.I64Value(0),
            ["action"] = new Value.TextValue(""),
            ["detail"] = new Value.TextValue(""),
            ["version"] = new Value.I64Value(0)
        }
    }, new Dictionary<string, IReadOnlyDictionary<string, bool>>
    {
       ["Platform"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["name"] = true,
           ["founded"] = true,
           ["user_email"] = true,
           ["version"] = true
        },
       ["TaskStatus"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["name"] = true,
           ["code"] = true,
           ["color"] = true,
           ["display_order"] = true,
           ["progress"] = true,
           ["platform"] = true,
           ["version"] = true
        },
       ["Task"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["name"] = true,
           ["status"] = true,
           ["platform"] = true,
           ["version"] = true
        },
       ["TaskExecutionLog"] = new Dictionary<string, bool> {
           ["id"] = true,
           ["task"] = true,
           ["action"] = true,
           ["detail"] = true,
           ["version"] = true
        }
    }, new Dictionary<string, IReadOnlyList<RelationDescriptor>>
    {
       ["Platform"] = new List<RelationDescriptor> {
            RelationDescriptor.New("TaskStatusList", "TaskStatus").LocalKey("id").ForeignKey("platform").Many(),
            RelationDescriptor.New("TaskList", "Task").LocalKey("id").ForeignKey("platform").Many()
        },
       ["TaskStatus"] = new List<RelationDescriptor> {
            RelationDescriptor.New("Platform", "Platform").LocalKey("platform").ForeignKey("id"),
            RelationDescriptor.New("TaskList", "Task").LocalKey("id").ForeignKey("status").Many()
        },
       ["Task"] = new List<RelationDescriptor> {
            RelationDescriptor.New("Status", "TaskStatus").LocalKey("status").ForeignKey("id"),
            RelationDescriptor.New("Platform", "Platform").LocalKey("platform").ForeignKey("id"),
            RelationDescriptor.New("TaskExecutionLogList", "TaskExecutionLog").LocalKey("id").ForeignKey("task").Many()
        },
       ["TaskExecutionLog"] = new List<RelationDescriptor> {
            RelationDescriptor.New("Task", "Task").LocalKey("task").ForeignKey("id")
        }
    }, new Dictionary<string, string>
    {
       ["Platform"] = "platform_data",
       ["TaskStatus"] = "task_status_data",
       ["Task"] = "task_data",
       ["TaskExecutionLog"] = "task_execution_log_data"
    }, integer32Fields: new Dictionary<string, IReadOnlyList<string>>
    {
       ["Platform"] = new List<string> {
        },
       ["TaskStatus"] = new List<string> {
        },
       ["Task"] = new List<string> {
        },
       ["TaskExecutionLog"] = new List<string> {
        }
    }).WireEntity(WireFields.CreateMetadata("Platform", ["id", "name", "founded", "user_email", "version"], profile: JsonFieldNamingProfiles.Parse("camelCase"), aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["name"] = ["name"], ["founded"] = ["founded"], ["user_email"] = ["user_email"], ["version"] = ["version"] })).WireEntity(WireFields.CreateMetadata("TaskStatus", ["id", "name", "code", "color", "display_order", "progress", "platform", "version"], profile: JsonFieldNamingProfiles.Parse("camelCase"), aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["name"] = ["name"], ["code"] = ["code"], ["color"] = ["color"], ["display_order"] = ["display_order"], ["progress"] = ["progress"], ["platform"] = ["platform"], ["version"] = ["version"] })).WireEntity(WireFields.CreateMetadata("Task", ["id", "name", "status", "platform", "version"], profile: JsonFieldNamingProfiles.Parse("camelCase"), aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["name"] = ["name"], ["status"] = ["status"], ["platform"] = ["platform"], ["version"] = ["version"] })).WireEntity(WireFields.CreateMetadata("TaskExecutionLog", ["id", "task", "action", "detail", "version"], profile: JsonFieldNamingProfiles.Parse("camelCase"), aliases: new Dictionary<string, IReadOnlyList<string>> { ["id"] = ["id"], ["task"] = ["task"], ["action"] = ["action"], ["detail"] = ["detail"], ["version"] = ["version"] })).GeneratedBootstrap(EnsureGeneratedBootstrapAsync);

    private static async Task EnsureGeneratedBootstrapAsync(UserContext context)
    {
        Exception? lastError = null;
        for (var attempt = 0; attempt < 5; attempt++)
        {
            try { await EnsureGeneratedBootstrapOnceAsync(context); return; }
            catch (Exception error) { lastError = error; if (attempt < 4) await Task.Delay((attempt + 1) * 10); }
        }
        throw lastError!;
    }

    private static async Task EnsureGeneratedBootstrapOnceAsync(UserContext context)
    {
        {
            using var bootstrapScope = context.EnterGeneratedBootstrap("Platform", 1);
        var domainRoot = await Q.Platforms().WithIdIs(1).Comment("what: locate generated Domain Root").Purpose("why: idempotent runtime bootstrap").ExecuteForOneAsync(context);
        if (domainRoot == null)
        {
            var created = Generated.Models.Platform.TeaqlNewWithFixedId(1);
            created.UpdateName("Robot System");
            created.UpdateUserEmail("admin@robots.example");
            try { domainRoot = await created.AuditAs("create generated Domain Root Platform").SaveAsync(context); }
            catch { domainRoot = await Q.Platforms().WithIdIs(1).Comment("what: recover concurrent Domain Root bootstrap").Purpose("why: make bootstrap idempotent").ExecuteForOneAsync(context); if (domainRoot == null) throw; }
        }
        var constantTaskStatus1001 = await Q.TaskStatuses().WithIdIs(1001).Comment("what: locate generated constant").Purpose("why: idempotent runtime bootstrap").ExecuteForOneAsync(context);
        if (constantTaskStatus1001 == null)
        {
            var created = Generated.Models.TaskStatus.TeaqlNewWithFixedId(1001);
            created.UpdateName("Planned");
            created.UpdateCode("PLANNED");
            created.UpdateColor("#94A3B8");
            created.UpdateDisplayOrder(decimal.Parse("10", System.Globalization.CultureInfo.InvariantCulture));
            created.UpdateProgress(decimal.Parse("0", System.Globalization.CultureInfo.InvariantCulture));
            created.UpdatePlatform(domainRoot.Id);
            try { constantTaskStatus1001 = await created.AuditAs("create model constant TaskStatus(1001)").SaveAsync(context); }
            catch { constantTaskStatus1001 = await Q.TaskStatuses().WithIdIs(1001).Comment("what: recover concurrent constant bootstrap").Purpose("why: make bootstrap idempotent").ExecuteForOneAsync(context); if (constantTaskStatus1001 == null) throw; }
        }
        else
        {
            var changed = false;
            if (!object.Equals(constantTaskStatus1001.Name, "Planned"))
            {
                constantTaskStatus1001.UpdateName("Planned");
                changed = true;
            }
            if (!object.Equals(constantTaskStatus1001.Code, "PLANNED"))
            {
                constantTaskStatus1001.UpdateCode("PLANNED");
                changed = true;
            }
            if (!object.Equals(constantTaskStatus1001.Color, "#94A3B8"))
            {
                constantTaskStatus1001.UpdateColor("#94A3B8");
                changed = true;
            }
            if (!object.Equals(constantTaskStatus1001.DisplayOrder, decimal.Parse("10", System.Globalization.CultureInfo.InvariantCulture)))
            {
                constantTaskStatus1001.UpdateDisplayOrder(decimal.Parse("10", System.Globalization.CultureInfo.InvariantCulture));
                changed = true;
            }
            if (!object.Equals(constantTaskStatus1001.Progress, decimal.Parse("0", System.Globalization.CultureInfo.InvariantCulture)))
            {
                constantTaskStatus1001.UpdateProgress(decimal.Parse("0", System.Globalization.CultureInfo.InvariantCulture));
                changed = true;
            }
            if (!object.Equals(constantTaskStatus1001.Platform, domainRoot.Id))
            {
                constantTaskStatus1001.UpdatePlatform(domainRoot.Id);
                changed = true;
            }
            if (changed) constantTaskStatus1001 = await constantTaskStatus1001.AuditAs("reconcile model constant TaskStatus(1001)").SaveAsync(context);
        }
        var constantTaskStatus1002 = await Q.TaskStatuses().WithIdIs(1002).Comment("what: locate generated constant").Purpose("why: idempotent runtime bootstrap").ExecuteForOneAsync(context);
        if (constantTaskStatus1002 == null)
        {
            var created = Generated.Models.TaskStatus.TeaqlNewWithFixedId(1002);
            created.UpdateName("Ready");
            created.UpdateCode("READY");
            created.UpdateColor("#3B82F6");
            created.UpdateDisplayOrder(decimal.Parse("20", System.Globalization.CultureInfo.InvariantCulture));
            created.UpdateProgress(decimal.Parse("25", System.Globalization.CultureInfo.InvariantCulture));
            created.UpdatePlatform(domainRoot.Id);
            try { constantTaskStatus1002 = await created.AuditAs("create model constant TaskStatus(1002)").SaveAsync(context); }
            catch { constantTaskStatus1002 = await Q.TaskStatuses().WithIdIs(1002).Comment("what: recover concurrent constant bootstrap").Purpose("why: make bootstrap idempotent").ExecuteForOneAsync(context); if (constantTaskStatus1002 == null) throw; }
        }
        else
        {
            var changed = false;
            if (!object.Equals(constantTaskStatus1002.Name, "Ready"))
            {
                constantTaskStatus1002.UpdateName("Ready");
                changed = true;
            }
            if (!object.Equals(constantTaskStatus1002.Code, "READY"))
            {
                constantTaskStatus1002.UpdateCode("READY");
                changed = true;
            }
            if (!object.Equals(constantTaskStatus1002.Color, "#3B82F6"))
            {
                constantTaskStatus1002.UpdateColor("#3B82F6");
                changed = true;
            }
            if (!object.Equals(constantTaskStatus1002.DisplayOrder, decimal.Parse("20", System.Globalization.CultureInfo.InvariantCulture)))
            {
                constantTaskStatus1002.UpdateDisplayOrder(decimal.Parse("20", System.Globalization.CultureInfo.InvariantCulture));
                changed = true;
            }
            if (!object.Equals(constantTaskStatus1002.Progress, decimal.Parse("25", System.Globalization.CultureInfo.InvariantCulture)))
            {
                constantTaskStatus1002.UpdateProgress(decimal.Parse("25", System.Globalization.CultureInfo.InvariantCulture));
                changed = true;
            }
            if (!object.Equals(constantTaskStatus1002.Platform, domainRoot.Id))
            {
                constantTaskStatus1002.UpdatePlatform(domainRoot.Id);
                changed = true;
            }
            if (changed) constantTaskStatus1002 = await constantTaskStatus1002.AuditAs("reconcile model constant TaskStatus(1002)").SaveAsync(context);
        }
        var constantTaskStatus1003 = await Q.TaskStatuses().WithIdIs(1003).Comment("what: locate generated constant").Purpose("why: idempotent runtime bootstrap").ExecuteForOneAsync(context);
        if (constantTaskStatus1003 == null)
        {
            var created = Generated.Models.TaskStatus.TeaqlNewWithFixedId(1003);
            created.UpdateName("Executing");
            created.UpdateCode("EXECUTING");
            created.UpdateColor("#F59E0B");
            created.UpdateDisplayOrder(decimal.Parse("30", System.Globalization.CultureInfo.InvariantCulture));
            created.UpdateProgress(decimal.Parse("50", System.Globalization.CultureInfo.InvariantCulture));
            created.UpdatePlatform(domainRoot.Id);
            try { constantTaskStatus1003 = await created.AuditAs("create model constant TaskStatus(1003)").SaveAsync(context); }
            catch { constantTaskStatus1003 = await Q.TaskStatuses().WithIdIs(1003).Comment("what: recover concurrent constant bootstrap").Purpose("why: make bootstrap idempotent").ExecuteForOneAsync(context); if (constantTaskStatus1003 == null) throw; }
        }
        else
        {
            var changed = false;
            if (!object.Equals(constantTaskStatus1003.Name, "Executing"))
            {
                constantTaskStatus1003.UpdateName("Executing");
                changed = true;
            }
            if (!object.Equals(constantTaskStatus1003.Code, "EXECUTING"))
            {
                constantTaskStatus1003.UpdateCode("EXECUTING");
                changed = true;
            }
            if (!object.Equals(constantTaskStatus1003.Color, "#F59E0B"))
            {
                constantTaskStatus1003.UpdateColor("#F59E0B");
                changed = true;
            }
            if (!object.Equals(constantTaskStatus1003.DisplayOrder, decimal.Parse("30", System.Globalization.CultureInfo.InvariantCulture)))
            {
                constantTaskStatus1003.UpdateDisplayOrder(decimal.Parse("30", System.Globalization.CultureInfo.InvariantCulture));
                changed = true;
            }
            if (!object.Equals(constantTaskStatus1003.Progress, decimal.Parse("50", System.Globalization.CultureInfo.InvariantCulture)))
            {
                constantTaskStatus1003.UpdateProgress(decimal.Parse("50", System.Globalization.CultureInfo.InvariantCulture));
                changed = true;
            }
            if (!object.Equals(constantTaskStatus1003.Platform, domainRoot.Id))
            {
                constantTaskStatus1003.UpdatePlatform(domainRoot.Id);
                changed = true;
            }
            if (changed) constantTaskStatus1003 = await constantTaskStatus1003.AuditAs("reconcile model constant TaskStatus(1003)").SaveAsync(context);
        }
        var constantTaskStatus1004 = await Q.TaskStatuses().WithIdIs(1004).Comment("what: locate generated constant").Purpose("why: idempotent runtime bootstrap").ExecuteForOneAsync(context);
        if (constantTaskStatus1004 == null)
        {
            var created = Generated.Models.TaskStatus.TeaqlNewWithFixedId(1004);
            created.UpdateName("Verified");
            created.UpdateCode("VERIFIED");
            created.UpdateColor("#16A34A");
            created.UpdateDisplayOrder(decimal.Parse("40", System.Globalization.CultureInfo.InvariantCulture));
            created.UpdateProgress(decimal.Parse("100", System.Globalization.CultureInfo.InvariantCulture));
            created.UpdatePlatform(domainRoot.Id);
            try { constantTaskStatus1004 = await created.AuditAs("create model constant TaskStatus(1004)").SaveAsync(context); }
            catch { constantTaskStatus1004 = await Q.TaskStatuses().WithIdIs(1004).Comment("what: recover concurrent constant bootstrap").Purpose("why: make bootstrap idempotent").ExecuteForOneAsync(context); if (constantTaskStatus1004 == null) throw; }
        }
        else
        {
            var changed = false;
            if (!object.Equals(constantTaskStatus1004.Name, "Verified"))
            {
                constantTaskStatus1004.UpdateName("Verified");
                changed = true;
            }
            if (!object.Equals(constantTaskStatus1004.Code, "VERIFIED"))
            {
                constantTaskStatus1004.UpdateCode("VERIFIED");
                changed = true;
            }
            if (!object.Equals(constantTaskStatus1004.Color, "#16A34A"))
            {
                constantTaskStatus1004.UpdateColor("#16A34A");
                changed = true;
            }
            if (!object.Equals(constantTaskStatus1004.DisplayOrder, decimal.Parse("40", System.Globalization.CultureInfo.InvariantCulture)))
            {
                constantTaskStatus1004.UpdateDisplayOrder(decimal.Parse("40", System.Globalization.CultureInfo.InvariantCulture));
                changed = true;
            }
            if (!object.Equals(constantTaskStatus1004.Progress, decimal.Parse("100", System.Globalization.CultureInfo.InvariantCulture)))
            {
                constantTaskStatus1004.UpdateProgress(decimal.Parse("100", System.Globalization.CultureInfo.InvariantCulture));
                changed = true;
            }
            if (!object.Equals(constantTaskStatus1004.Platform, domainRoot.Id))
            {
                constantTaskStatus1004.UpdatePlatform(domainRoot.Id);
                changed = true;
            }
            if (changed) constantTaskStatus1004 = await constantTaskStatus1004.AuditAs("reconcile model constant TaskStatus(1004)").SaveAsync(context);
        }
        }
        context.WithActiveRoot("Platform", 1);
    }


    static GeneratedRuntimeModule()
    {
        Module.Entity(Module.Metadata.GetEntity("Platform")!.AuditMaskFields(new List<string> {  }));
        Module.Entity(Module.Metadata.GetEntity("TaskStatus")!.AuditMaskFields(new List<string> {  }));
        Module.Entity(Module.Metadata.GetEntity("Task")!.AuditMaskFields(new List<string> {  }));
        Module.Entity(Module.Metadata.GetEntity("TaskExecutionLog")!.AuditMaskFields(new List<string> { "detail" }));
    }
}