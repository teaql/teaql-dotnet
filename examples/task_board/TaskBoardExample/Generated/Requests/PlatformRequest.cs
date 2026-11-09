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
    public class PlatformRequest
    {
        private SelectQuery _query;
        private string? _purpose;
        private string? _comment;
        private bool _countRequested;
        private readonly List<DynamicSearchWarning> _dynamicSearchWarnings = new();
        private static object TeaqlQueryValue(object value) =>
            value is TimeSpan time
                ? time.ToString("c", System.Globalization.CultureInfo.InvariantCulture)
                : value;

        public PlatformRequest()
        {
            _query = new SelectQuery("Platform");
            _query.Project("id");
            _query.Project("version");
            _query.AndFilter(new FilterExpression { Operator = "gt", Field = "version", Expected = 0L });
        }

        public SelectQuery GetQuery() => _query;

        public PlatformRequest FilterByJson(string source, Action<DynamicSearchWarning>? warn = null)
        {
            var result = DynamicSearch.MergeGeneratedScalar(_query, source, GeneratedSearchModels.Models, warn);
            _query = result.Query;
            _dynamicSearchWarnings.AddRange(result.Warnings);
            return this;
        }

        public IReadOnlyList<DynamicSearchWarning> DynamicSearchWarnings => _dynamicSearchWarnings.ToArray();

        public PlatformRequest WithDeletedRows()
        {
            _query.Filters.RemoveAll(filter => filter.Field == "version" && filter.Operator == "gt");
            return this;
        }

        public PlatformRequest DeletedRowsOnly()
        {
            WithDeletedRows();
            _query.AndFilter(new FilterExpression { Operator = "lt", Field = "version", Expected = 0L });
            return this;
        }

        public PlatformRequest Comment(string c)
        {
            _query.Comment(c);
            _comment = c;
            return this;
        }

        public ExecutablePlatformRequest Purpose(string p)
        {
            p = QueryIntent.RequirePurpose(p);
            _query.Purpose(p);
            _purpose = p;
            return new ExecutablePlatformRequest(
                ExecuteForListInternalAsync,
                ExecuteForOneInternalAsync,
                ExecuteForListWithMetadataInternalAsync,
                ExecuteForStreamInternalAsync,
                c => Comment(c),
                EnsureIntent);
        }

        public PlatformRequest OptimizeForContinuousPageFetch()
        {
            _query.OptimizeForContinuousPageFetch();
            return this;
        }

        public PlatformRequest OptimizeForContinuousPageFetchWith(string namespaceName, int ttlSeconds)
        {
            _query.OptimizeForContinuousPageFetchWith(namespaceName, ttlSeconds);
            return this;
        }

        public PlatformRequest OptimizePaginationWithIdSet()
        {
            _query.OptimizePaginationWithIdSet();
            return this;
        }

        public PlatformRequest OptimizePaginationWithIdSet(string namespaceName, int ttlSeconds, int maxIds)
        {
            _query.OptimizePaginationWithIdSet(namespaceName, ttlSeconds, maxIds);
            return this;
        }

        public PlatformRequest TopNProbeParentThreshold(int threshold)
        {
            _query.TopNProbeParentThreshold(threshold);
            return this;
        }

        public PlatformRequest Limit(int n)
        {
            _query.Limit(n);
            return this;
        }

        public PlatformRequest Offset(int n)
        {
            _query.Offset(n);
            return this;
        }

        public PlatformRequest SelectSelfFields()
        {
            _query.Project("id");
            _query.Project("name");
            _query.Project("founded");
            _query.Project("user_email");
            _query.Project("version");
            return this;
        }

                public PlatformRequest SelectId()
                {
                    _query.Project("id");
                    return this;
                }

                public PlatformRequest SelectName()
                {
                    _query.Project("name");
                    return this;
                }

                public PlatformRequest SelectFounded()
                {
                    _query.Project("founded");
                    return this;
                }

                public PlatformRequest SelectUserEmail()
                {
                    _query.Project("user_email");
                    return this;
                }

                public PlatformRequest SelectVersion()
                {
                    _query.Project("version");
                    return this;
                }


                public PlatformRequest WithIdIs(long val)
                {
                    _query.AndFilter(Expr.Eq("id", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithIdIsNot(long val)
                {
                    _query.AndFilter(Expr.Ne("id", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithIdIn(params long[] vals)
                {
                    _query.AndFilter(Expr.In("id", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public PlatformRequest WithIdNotIn(params long[] vals)
                {
                    _query.AndFilter(Expr.NotIn("id", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public PlatformRequest WithIdGreaterThan(long val)
                {
                    _query.AndFilter(Expr.Gt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithIdGreaterThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Gte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithIdLessThan(long val)
                {
                    _query.AndFilter(Expr.Lt("id", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithIdLessThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Lte("id", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithIdBetween(long lower, long upper)
                {
                    _query.AndFilter(Expr.Between("id", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public PlatformRequest WithIdIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("id"));
                    return this;
                }

                public PlatformRequest WithIdIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("id"));
                    return this;
                }

                public PlatformRequest WithNameContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("name", val));
                    return this;
                }

                public PlatformRequest WithNameIs(string val)
                {
                    _query.AndFilter(Expr.Eq("name", TeaqlQueryValue(val)));
                    return this;
                }
                public PlatformRequest WithNameIsNot(string val)
                {
                    _query.AndFilter(Expr.Ne("name", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithNameIn(params string[] vals)
                {
                    _query.AndFilter(Expr.In("name", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public PlatformRequest WithNameNotIn(params string[] vals)
                {
                    _query.AndFilter(Expr.NotIn("name", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public PlatformRequest WithNameGreaterThan(string val)
                {
                    _query.AndFilter(Expr.Gt("name", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithNameGreaterThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Gte("name", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithNameLessThan(string val)
                {
                    _query.AndFilter(Expr.Lt("name", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithNameLessThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Lte("name", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithNameBetween(string lower, string upper)
                {
                    _query.AndFilter(Expr.Between("name", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public PlatformRequest WithNameIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("name"));
                    return this;
                }

                public PlatformRequest WithNameIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("name"));
                    return this;
                }
                public PlatformRequest WithNameNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("name", val));
                    return this;
                }

                public PlatformRequest WithNameStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("name", val));
                    return this;
                }

                public PlatformRequest WithNameNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("name", val));
                    return this;
                }

                public PlatformRequest WithNameEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("name", val));
                    return this;
                }

                public PlatformRequest WithNameNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("name", val));
                    return this;
                }

                public PlatformRequest WithNameSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("name", val));
                    return this;
                }

                public PlatformRequest WithFoundedIs(long val)
                {
                    _query.AndFilter(Expr.Eq("founded", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithFoundedIsNot(long val)
                {
                    _query.AndFilter(Expr.Ne("founded", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithFoundedIn(params long[] vals)
                {
                    _query.AndFilter(Expr.In("founded", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public PlatformRequest WithFoundedNotIn(params long[] vals)
                {
                    _query.AndFilter(Expr.NotIn("founded", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public PlatformRequest WithFoundedGreaterThan(long val)
                {
                    _query.AndFilter(Expr.Gt("founded", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithFoundedGreaterThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Gte("founded", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithFoundedLessThan(long val)
                {
                    _query.AndFilter(Expr.Lt("founded", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithFoundedLessThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Lte("founded", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithFoundedBetween(long lower, long upper)
                {
                    _query.AndFilter(Expr.Between("founded", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public PlatformRequest WithFoundedIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("founded"));
                    return this;
                }

                public PlatformRequest WithFoundedIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("founded"));
                    return this;
                }

                public PlatformRequest WithUserEmailContaining(string val)
                {
                    _query.AndFilter(Expr.Contain("user_email", val));
                    return this;
                }

                public PlatformRequest WithUserEmailIs(string val)
                {
                    _query.AndFilter(Expr.Eq("user_email", TeaqlQueryValue(val)));
                    return this;
                }
                public PlatformRequest WithUserEmailIsNot(string val)
                {
                    _query.AndFilter(Expr.Ne("user_email", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithUserEmailIn(params string[] vals)
                {
                    _query.AndFilter(Expr.In("user_email", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public PlatformRequest WithUserEmailNotIn(params string[] vals)
                {
                    _query.AndFilter(Expr.NotIn("user_email", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public PlatformRequest WithUserEmailGreaterThan(string val)
                {
                    _query.AndFilter(Expr.Gt("user_email", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithUserEmailGreaterThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Gte("user_email", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithUserEmailLessThan(string val)
                {
                    _query.AndFilter(Expr.Lt("user_email", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithUserEmailLessThanOrEqualTo(string val)
                {
                    _query.AndFilter(Expr.Lte("user_email", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithUserEmailBetween(string lower, string upper)
                {
                    _query.AndFilter(Expr.Between("user_email", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public PlatformRequest WithUserEmailIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("user_email"));
                    return this;
                }

                public PlatformRequest WithUserEmailIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("user_email"));
                    return this;
                }
                public PlatformRequest WithUserEmailNotContaining(string val)
                {
                    _query.AndFilter(Expr.NotContain("user_email", val));
                    return this;
                }

                public PlatformRequest WithUserEmailStartingWith(string val)
                {
                    _query.AndFilter(Expr.BeginWith("user_email", val));
                    return this;
                }

                public PlatformRequest WithUserEmailNotStartingWith(string val)
                {
                    _query.AndFilter(Expr.NotBeginWith("user_email", val));
                    return this;
                }

                public PlatformRequest WithUserEmailEndingWith(string val)
                {
                    _query.AndFilter(Expr.EndWith("user_email", val));
                    return this;
                }

                public PlatformRequest WithUserEmailNotEndingWith(string val)
                {
                    _query.AndFilter(Expr.NotEndWith("user_email", val));
                    return this;
                }

                public PlatformRequest WithUserEmailSoundingLike(string val)
                {
                    _query.AndFilter(Expr.SoundLike("user_email", val));
                    return this;
                }

                public PlatformRequest WithVersionIs(long val)
                {
                    _query.AndFilter(Expr.Eq("version", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithVersionIsNot(long val)
                {
                    _query.AndFilter(Expr.Ne("version", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithVersionIn(params long[] vals)
                {
                    _query.AndFilter(Expr.In("version", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public PlatformRequest WithVersionNotIn(params long[] vals)
                {
                    _query.AndFilter(Expr.NotIn("version", vals.Select(val => TeaqlQueryValue(val)).ToArray()));
                    return this;
                }

                public PlatformRequest WithVersionGreaterThan(long val)
                {
                    _query.AndFilter(Expr.Gt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithVersionGreaterThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Gte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithVersionLessThan(long val)
                {
                    _query.AndFilter(Expr.Lt("version", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithVersionLessThanOrEqualTo(long val)
                {
                    _query.AndFilter(Expr.Lte("version", TeaqlQueryValue(val)));
                    return this;
                }

                public PlatformRequest WithVersionBetween(long lower, long upper)
                {
                    _query.AndFilter(Expr.Between("version", TeaqlQueryValue(lower), TeaqlQueryValue(upper)));
                    return this;
                }

                public PlatformRequest WithVersionIsKnown()
                {
                    _query.AndFilter(Expr.IsNotNull("version"));
                    return this;
                }

                public PlatformRequest WithVersionIsUnknown()
                {
                    _query.AndFilter(Expr.IsNull("version"));
                    return this;
                }

                public PlatformRequest OrderByIdAscending()
                {
                    _query.OrderBy("id", "asc");
                    return this;
                }

                public PlatformRequest OrderByIdDescending()
                {
                    _query.OrderBy("id", "desc");
                    return this;
                }

                public PlatformRequest OrderByNameAscending()
                {
                    _query.OrderBy("name", "asc");
                    return this;
                }

                public PlatformRequest OrderByNameDescending()
                {
                    _query.OrderBy("name", "desc");
                    return this;
                }

                public PlatformRequest OrderByFoundedAscending()
                {
                    _query.OrderBy("founded", "asc");
                    return this;
                }

                public PlatformRequest OrderByFoundedDescending()
                {
                    _query.OrderBy("founded", "desc");
                    return this;
                }

                public PlatformRequest OrderByUserEmailAscending()
                {
                    _query.OrderBy("user_email", "asc");
                    return this;
                }

                public PlatformRequest OrderByUserEmailDescending()
                {
                    _query.OrderBy("user_email", "desc");
                    return this;
                }

                public PlatformRequest OrderByVersionAscending()
                {
                    _query.OrderBy("version", "asc");
                    return this;
                }

                public PlatformRequest OrderByVersionDescending()
                {
                    _query.OrderBy("version", "desc");
                    return this;
                }


        public PlatformRequest Count()
        {
            return CountAs("count");
        }

        public PlatformRequest CountAs(string retName)
        {
            _countRequested = true;
            _query.Aggregate("Count", "id", retName);
            _query.PageCountOperation = new QueryOperation("count", "CountAs", retName);
            return this;
        }

                public PlatformRequest GroupById()
                {
                    _query.GroupBy("id");
                    return this;
                }

                public PlatformRequest GroupByIdAs(string retName)
                {
                    _query.GroupBy("id");
                    _query.ProjectExpr(retName, Expr.Column("id"));
                    return this;
                }
                public PlatformRequest GroupByName()
                {
                    _query.GroupBy("name");
                    return this;
                }

                public PlatformRequest GroupByNameAs(string retName)
                {
                    _query.GroupBy("name");
                    _query.ProjectExpr(retName, Expr.Column("name"));
                    return this;
                }
                public PlatformRequest GroupByFounded()
                {
                    _query.GroupBy("founded");
                    return this;
                }

                public PlatformRequest GroupByFoundedAs(string retName)
                {
                    _query.GroupBy("founded");
                    _query.ProjectExpr(retName, Expr.Column("founded"));
                    return this;
                }
                public PlatformRequest GroupByUserEmail()
                {
                    _query.GroupBy("user_email");
                    return this;
                }

                public PlatformRequest GroupByUserEmailAs(string retName)
                {
                    _query.GroupBy("user_email");
                    _query.ProjectExpr(retName, Expr.Column("user_email"));
                    return this;
                }
                public PlatformRequest GroupByVersion()
                {
                    _query.GroupBy("version");
                    return this;
                }

                public PlatformRequest GroupByVersionAs(string retName)
                {
                    _query.GroupBy("version");
                    _query.ProjectExpr(retName, Expr.Column("version"));
                    return this;
                }
                public PlatformRequest SelectTaskStatusList()
                {
                    return SelectTaskStatusListWith(new TaskStatusRequest());
                }

                public PlatformRequest SelectTaskStatusListWith(TaskStatusRequest child)
                {
                    _query.RelationQuery("TaskStatusList", "TaskStatus", "platform", true, child.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectTaskStatusListWith", Relation: "TaskStatusList")
                    };
                    return this;
                }
                public PlatformRequest SelectTaskList()
                {
                    return SelectTaskListWith(new TaskRequest());
                }

                public PlatformRequest SelectTaskListWith(TaskRequest child)
                {
                    _query.RelationQuery("TaskList", "Task", "platform", true, child.GetQuery());
                    _query.RelationLoads[^1] = _query.RelationLoads[^1] with {
                        Operation = new QueryOperation("relation", "SelectTaskListWith", Relation: "TaskList")
                    };
                    return this;
                }
                public PlatformRequest HaveTaskStatuses()
                    => WithTaskStatusListMatching(new TaskStatusRequest());

                public PlatformRequest HaveNoTaskStatuses()
                    => WithoutTaskStatusListMatching(new TaskStatusRequest());

                public PlatformRequest WithTaskStatusListMatching(TaskStatusRequest child)
                {
                    _query.AndFilter(Expr.InSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("TaskStatus")!, child.GetQuery(), "platform"));
                    return this;
                }

                public PlatformRequest WithoutTaskStatusListMatching(TaskStatusRequest child)
                {
                    _query.AndFilter(Expr.NotInSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("TaskStatus")!, child.GetQuery(), "platform"));
                    return this;
                }
                public PlatformRequest HaveTasks()
                    => WithTaskListMatching(new TaskRequest());

                public PlatformRequest HaveNoTasks()
                    => WithoutTaskListMatching(new TaskRequest());

                public PlatformRequest WithTaskListMatching(TaskRequest child)
                {
                    _query.AndFilter(Expr.InSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("Task")!, child.GetQuery(), "platform"));
                    return this;
                }

                public PlatformRequest WithoutTaskListMatching(TaskRequest child)
                {
                    _query.AndFilter(Expr.NotInSubquery("id", GeneratedRuntimeModule.Module.Metadata.GetEntity("Task")!, child.GetQuery(), "platform"));
                    return this;
                }
                public PlatformRequest CountTaskStatuses()
                    => CountTaskStatusesAs("countTaskStatuses");

                public PlatformRequest CountTaskStatusesAs(string alias)
                {
                    CountTaskStatusesWith(alias, new TaskStatusRequest());
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountTaskStatusesAs", alias, "TaskStatusList")
                    };
                    return this;
                }

                public PlatformRequest CountTaskStatusesWith(string alias, TaskStatusRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.CountField("id", alias);
                    _query.RelationAggregate("TaskStatusList", "TaskStatus", "platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountTaskStatusesWith", alias, "TaskStatusList")
                    };
                    return this;
                }

                public PlatformRequest MinDisplayOrderOfTaskStatuses()
                    => MinDisplayOrderOfTaskStatusesAs("minOfDisplayOrderOfTaskStatuses", new TaskStatusRequest());

                public PlatformRequest MinDisplayOrderOfTaskStatusesAs(string alias, TaskStatusRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("min", "display_order", alias);
                    _query.RelationAggregate("TaskStatusList", "TaskStatus", "platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MinDisplayOrderOfTaskStatusesAs", alias, "TaskStatusList")
                    };
                    return this;
                }
                public PlatformRequest MaxDisplayOrderOfTaskStatuses()
                    => MaxDisplayOrderOfTaskStatusesAs("maxOfDisplayOrderOfTaskStatuses", new TaskStatusRequest());

                public PlatformRequest MaxDisplayOrderOfTaskStatusesAs(string alias, TaskStatusRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("max", "display_order", alias);
                    _query.RelationAggregate("TaskStatusList", "TaskStatus", "platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MaxDisplayOrderOfTaskStatusesAs", alias, "TaskStatusList")
                    };
                    return this;
                }
                public PlatformRequest SumDisplayOrderOfTaskStatuses()
                    => SumDisplayOrderOfTaskStatusesAs("sumOfDisplayOrderOfTaskStatuses", new TaskStatusRequest());

                public PlatformRequest SumDisplayOrderOfTaskStatusesAs(string alias, TaskStatusRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("sum", "display_order", alias);
                    _query.RelationAggregate("TaskStatusList", "TaskStatus", "platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SumDisplayOrderOfTaskStatusesAs", alias, "TaskStatusList")
                    };
                    return this;
                }
                public PlatformRequest AvgDisplayOrderOfTaskStatuses()
                    => AvgDisplayOrderOfTaskStatusesAs("avgOfDisplayOrderOfTaskStatuses", new TaskStatusRequest());

                public PlatformRequest AvgDisplayOrderOfTaskStatusesAs(string alias, TaskStatusRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("avg", "display_order", alias);
                    _query.RelationAggregate("TaskStatusList", "TaskStatus", "platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "AvgDisplayOrderOfTaskStatusesAs", alias, "TaskStatusList")
                    };
                    return this;
                }
                public PlatformRequest StandardDeviationDisplayOrderOfTaskStatuses()
                    => StandardDeviationDisplayOrderOfTaskStatusesAs("standardDeviationOfDisplayOrderOfTaskStatuses", new TaskStatusRequest());

                public PlatformRequest StandardDeviationDisplayOrderOfTaskStatusesAs(string alias, TaskStatusRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev", "display_order", alias);
                    _query.RelationAggregate("TaskStatusList", "TaskStatus", "platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "StandardDeviationDisplayOrderOfTaskStatusesAs", alias, "TaskStatusList")
                    };
                    return this;
                }
                public PlatformRequest SquareRootOfPopulationStandardDeviationDisplayOrderOfTaskStatuses()
                    => SquareRootOfPopulationStandardDeviationDisplayOrderOfTaskStatusesAs("squareRootOfPopulationStandardDeviationOfDisplayOrderOfTaskStatuses", new TaskStatusRequest());

                public PlatformRequest SquareRootOfPopulationStandardDeviationDisplayOrderOfTaskStatusesAs(string alias, TaskStatusRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev_pop", "display_order", alias);
                    _query.RelationAggregate("TaskStatusList", "TaskStatus", "platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SquareRootOfPopulationStandardDeviationDisplayOrderOfTaskStatusesAs", alias, "TaskStatusList")
                    };
                    return this;
                }
                public PlatformRequest SampleVarianceDisplayOrderOfTaskStatuses()
                    => SampleVarianceDisplayOrderOfTaskStatusesAs("sampleVarianceOfDisplayOrderOfTaskStatuses", new TaskStatusRequest());

                public PlatformRequest SampleVarianceDisplayOrderOfTaskStatusesAs(string alias, TaskStatusRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_samp", "display_order", alias);
                    _query.RelationAggregate("TaskStatusList", "TaskStatus", "platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SampleVarianceDisplayOrderOfTaskStatusesAs", alias, "TaskStatusList")
                    };
                    return this;
                }
                public PlatformRequest SamplePopulationVarianceDisplayOrderOfTaskStatuses()
                    => SamplePopulationVarianceDisplayOrderOfTaskStatusesAs("samplePopulationVarianceOfDisplayOrderOfTaskStatuses", new TaskStatusRequest());

                public PlatformRequest SamplePopulationVarianceDisplayOrderOfTaskStatusesAs(string alias, TaskStatusRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_pop", "display_order", alias);
                    _query.RelationAggregate("TaskStatusList", "TaskStatus", "platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SamplePopulationVarianceDisplayOrderOfTaskStatusesAs", alias, "TaskStatusList")
                    };
                    return this;
                }
                public PlatformRequest MinProgressOfTaskStatuses()
                    => MinProgressOfTaskStatusesAs("minOfProgressOfTaskStatuses", new TaskStatusRequest());

                public PlatformRequest MinProgressOfTaskStatusesAs(string alias, TaskStatusRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("min", "progress", alias);
                    _query.RelationAggregate("TaskStatusList", "TaskStatus", "platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MinProgressOfTaskStatusesAs", alias, "TaskStatusList")
                    };
                    return this;
                }
                public PlatformRequest MaxProgressOfTaskStatuses()
                    => MaxProgressOfTaskStatusesAs("maxOfProgressOfTaskStatuses", new TaskStatusRequest());

                public PlatformRequest MaxProgressOfTaskStatusesAs(string alias, TaskStatusRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("max", "progress", alias);
                    _query.RelationAggregate("TaskStatusList", "TaskStatus", "platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "MaxProgressOfTaskStatusesAs", alias, "TaskStatusList")
                    };
                    return this;
                }
                public PlatformRequest SumProgressOfTaskStatuses()
                    => SumProgressOfTaskStatusesAs("sumOfProgressOfTaskStatuses", new TaskStatusRequest());

                public PlatformRequest SumProgressOfTaskStatusesAs(string alias, TaskStatusRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("sum", "progress", alias);
                    _query.RelationAggregate("TaskStatusList", "TaskStatus", "platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SumProgressOfTaskStatusesAs", alias, "TaskStatusList")
                    };
                    return this;
                }
                public PlatformRequest AvgProgressOfTaskStatuses()
                    => AvgProgressOfTaskStatusesAs("avgOfProgressOfTaskStatuses", new TaskStatusRequest());

                public PlatformRequest AvgProgressOfTaskStatusesAs(string alias, TaskStatusRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("avg", "progress", alias);
                    _query.RelationAggregate("TaskStatusList", "TaskStatus", "platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "AvgProgressOfTaskStatusesAs", alias, "TaskStatusList")
                    };
                    return this;
                }
                public PlatformRequest StandardDeviationProgressOfTaskStatuses()
                    => StandardDeviationProgressOfTaskStatusesAs("standardDeviationOfProgressOfTaskStatuses", new TaskStatusRequest());

                public PlatformRequest StandardDeviationProgressOfTaskStatusesAs(string alias, TaskStatusRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev", "progress", alias);
                    _query.RelationAggregate("TaskStatusList", "TaskStatus", "platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "StandardDeviationProgressOfTaskStatusesAs", alias, "TaskStatusList")
                    };
                    return this;
                }
                public PlatformRequest SquareRootOfPopulationStandardDeviationProgressOfTaskStatuses()
                    => SquareRootOfPopulationStandardDeviationProgressOfTaskStatusesAs("squareRootOfPopulationStandardDeviationOfProgressOfTaskStatuses", new TaskStatusRequest());

                public PlatformRequest SquareRootOfPopulationStandardDeviationProgressOfTaskStatusesAs(string alias, TaskStatusRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("stddev_pop", "progress", alias);
                    _query.RelationAggregate("TaskStatusList", "TaskStatus", "platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SquareRootOfPopulationStandardDeviationProgressOfTaskStatusesAs", alias, "TaskStatusList")
                    };
                    return this;
                }
                public PlatformRequest SampleVarianceProgressOfTaskStatuses()
                    => SampleVarianceProgressOfTaskStatusesAs("sampleVarianceOfProgressOfTaskStatuses", new TaskStatusRequest());

                public PlatformRequest SampleVarianceProgressOfTaskStatusesAs(string alias, TaskStatusRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_samp", "progress", alias);
                    _query.RelationAggregate("TaskStatusList", "TaskStatus", "platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SampleVarianceProgressOfTaskStatusesAs", alias, "TaskStatusList")
                    };
                    return this;
                }
                public PlatformRequest SamplePopulationVarianceProgressOfTaskStatuses()
                    => SamplePopulationVarianceProgressOfTaskStatusesAs("samplePopulationVarianceOfProgressOfTaskStatuses", new TaskStatusRequest());

                public PlatformRequest SamplePopulationVarianceProgressOfTaskStatusesAs(string alias, TaskStatusRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.Aggregate("var_pop", "progress", alias);
                    _query.RelationAggregate("TaskStatusList", "TaskStatus", "platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("aggregate", "SamplePopulationVarianceProgressOfTaskStatusesAs", alias, "TaskStatusList")
                    };
                    return this;
                }
                public PlatformRequest CountTasks()
                    => CountTasksAs("countTasks");

                public PlatformRequest CountTasksAs(string alias)
                {
                    CountTasksWith(alias, new TaskRequest());
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountTasksAs", alias, "TaskList")
                    };
                    return this;
                }

                public PlatformRequest CountTasksWith(string alias, TaskRequest child)
                {
                    var query = child.GetQuery().CloneForExecution();
                    query.AggregateItems.Clear(); query.CountField("id", alias);
                    _query.RelationAggregate("TaskList", "Task", "platform", alias, query, true);
                    _query.RelationAggregates[^1] = _query.RelationAggregates[^1] with {
                        Operation = new QueryOperation("count", "CountTasksWith", alias, "TaskList")
                    };
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

        private async Task<Generated.Models.Platform?> ExecuteForOneInternalAsync(UserContext context)
        {
            EnsureIntent();
            var service = context.RequireResource<IDataService>();
            var query = _query.CloneForExecution();
            query.Limit(1);
            var req = context.PrepareQueryRequest(new QueryRequest(query, new QueryIntent(_comment, _purpose)));
            var result = await service.QueryAsync(req);
            if (result.Rows.Count == 0) return null;
            return global::TeaQL.Runtime.EntityQueryProperties.Capture(
                Generated.Models.Platform.FromRecord(result.Rows[0], new EntityRoot()), req.Query);
        }

        private async Task<SmartList<Generated.Models.Platform>> ExecuteForListWithMetadataInternalAsync(
            UserContext context)
        {
            EnsureIntent();
            var service = context.RequireResource<IDataService>();
            var query = _query.CloneForExecution();
            var request = context.PrepareQueryRequest(new QueryRequest(query, new QueryIntent(_comment, _purpose)));
            SmartList<Record> result;
            if (_countRequested) result = await RuntimeDataService.FetchPageAsync(service, context, request);
            else {
                var rows = await service.QueryAsync(request);
                result = new SmartList<Record>(rows.Rows) { Facets = rows.Facets };
            }
            var entities = new SmartList<Generated.Models.Platform>() {
                TotalCount = result.TotalCount, Aggregations = result.Aggregations,
                Summary = result.Summary, Facets = result.Facets
            };
            foreach (var row in result)
                entities.Add(Generated.Models.Platform.FromRecord(row, new EntityRoot()));
            global::TeaQL.Runtime.EntityQueryProperties.CaptureList(entities, request.Query);
            return entities;
        }

        private IAsyncEnumerable<Generated.Models.Platform> ExecuteForStreamInternalAsync(
            UserContext context,
            int chunkSize,
            CancellationToken cancellationToken = default)
        {
            EnsureIntent();
            var service = context.RequireResource<IDataService>();
            if (service is not IStreamQueryExecutor streaming)
                throw new NotSupportedException("The configured data service does not provide a local streaming cursor; federation streaming requires a separate protocol");
            var request = context.PrepareQueryRequest(new QueryRequest(_query, new QueryIntent(_comment, _purpose)));
            return RuntimeDataService.StreamEntities(context, request,
                row => global::TeaQL.Runtime.EntityQueryProperties.Capture(
                    Generated.Models.Platform.FromRecord(row, new EntityRoot()), request.Query),
                chunkSize, cancellationToken);
        }

        private void EnsureIntent()
        {
            _ = new QueryIntent(_comment, _purpose);
        }

    }

    public sealed class ExecutablePlatformRequest
    {
        private readonly Func<UserContext, Task<QueryResult>> _executeForRows;
        private readonly Func<UserContext, Task<Generated.Models.Platform?>> _executeForOne;
        private readonly Func<UserContext, Task<SmartList<Generated.Models.Platform>>> _executeForList;
        private readonly Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.Platform>> _executeForStream;
        private readonly Action<string> _comment;
        private readonly Action _ensureIntent;

        internal ExecutablePlatformRequest(
            Func<UserContext, Task<QueryResult>> executeForRows,
            Func<UserContext, Task<Generated.Models.Platform?>> executeForOne,
            Func<UserContext, Task<SmartList<Generated.Models.Platform>>> executeForList,
            Func<UserContext, int, CancellationToken, IAsyncEnumerable<Generated.Models.Platform>> executeForStream,
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

        public ExecutablePlatformRequest Comment(string c)
        {
            _comment(c);
            return this;
        }

        public async Task<Generated.Models.Platform> NewEntityAsync(UserContext context)
        {
            _ensureIntent();
            ArgumentNullException.ThrowIfNull(context);
            var identity = checked((long)await context.NextIdAsync("Platform").ConfigureAwait(false));
            var entity = Generated.Models.Platform.TeaqlNewWithReservedId(identity);
            return context.InitializeEntity("Platform", entity,
                initialized => initialized.TeaqlValidateReservedIdentity(identity));
        }

        public Task<QueryResult> ExecuteForRowsAsync(UserContext context)
            => _executeForRows(context);

        public Task<SmartList<Generated.Models.Platform>> ExecuteForListAsync(UserContext context)
            => _executeForList(context);

        public IAsyncEnumerable<Generated.Models.Platform> ExecuteForStreamAsync(
            UserContext context,
            int chunkSize = 1000,
            CancellationToken cancellationToken = default)
            => _executeForStream(context, chunkSize, cancellationToken);

        public Task<Generated.Models.Platform?> ExecuteForOneAsync(
            UserContext context)
            => _executeForOne(context);
    }

}