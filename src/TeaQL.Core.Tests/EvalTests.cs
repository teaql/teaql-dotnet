using System;
using System.Collections.Generic;
using Xunit;
using TeaQL.Core;

namespace TeaQL.Core.Tests;

public class EvalTests
{
    private class Company
    {
        public string? Name { get; set; }
        public LoadState _LoadState { get; set; } = LoadState.NotLoaded;

        public EvalResult<string> EvalName()
        {
            if (!_LoadState.IsLoaded("Name"))
            {
                return EvalResult<string>.NotLoaded("Name", "Name");
            }
            return Name != null ? EvalResult<string>.Value(Name) : EvalResult<string>.Null;
        }
    }

    private class Platform
    {
        public Company? Company { get; set; }
        public LoadState _LoadState { get; set; } = LoadState.NotLoaded;

        public EvalResult<Company> EvalCompany()
        {
            if (!_LoadState.IsLoaded("Company"))
            {
                return EvalResult<Company>.NotLoaded("Company", "Company");
            }
            return Company != null ? EvalResult<Company>.Value(Company) : EvalResult<Company>.Null;
        }
    }

    private class User
    {
        public Platform? Platform { get; set; }
        public LoadState _LoadState { get; set; } = LoadState.NotLoaded;

        public EvalResult<Platform> EvalPlatform()
        {
            if (!_LoadState.IsLoaded("Platform"))
            {
                return EvalResult<Platform>.NotLoaded("Platform", "Platform");
            }
            return Platform != null ? EvalResult<Platform>.Value(Platform) : EvalResult<Platform>.Null;
        }
    }

    [Fact]
    public void TestEvalTrackingChainPerfectPath()
    {
        var company = new Company
        {
            Name = null,
            _LoadState = LoadState.NotLoaded
        };

        var platform = new Platform
        {
            Company = company,
            _LoadState = LoadState.FullyLoaded
        };

        var user = new User
        {
            Platform = platform,
            _LoadState = LoadState.FullyLoaded
        };

        var result = user.EvalPlatform().AndThen("Platform", p => 
            p.EvalCompany().AndThen("Company", c => c.EvalName())
        );

        var notLoaded = Assert.IsType<EvalResult<string>.NotLoadedResult>(result);
        Assert.Equal("Platform.Company.Name", notLoaded.AttemptedPath);
    }

    [Fact]
    public void TestEvalTrackingChainMiddleBreak()
    {
        var platform = new Platform
        {
            Company = null,
            _LoadState = LoadState.NotLoaded
        };

        var user = new User
        {
            Platform = platform,
            _LoadState = LoadState.FullyLoaded
        };

        var result = user.EvalPlatform().AndThen("Platform", p => 
            p.EvalCompany().AndThen("Company", c => c.EvalName())
        );

        var notLoaded = Assert.IsType<EvalResult<string>.NotLoadedResult>(result);
        Assert.Equal("Platform.Company", notLoaded.AttemptedPath);
    }

    [Fact]
    public void TestEvalTrackingChainNormalNull()
    {
        var company = new Company
        {
            Name = null,
            _LoadState = LoadState.FullyLoaded
        };

        var platform = new Platform
        {
            Company = company,
            _LoadState = LoadState.FullyLoaded
        };

        var user = new User
        {
            Platform = platform,
            _LoadState = LoadState.FullyLoaded
        };

        var result = user.EvalPlatform().AndThen("Platform", p => 
            p.EvalCompany().AndThen("Company", c => c.EvalName())
        );

        Assert.IsType<EvalResult<string>.NullResult>(result);
    }
}
