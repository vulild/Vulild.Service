using System;
using System.Collections.Generic;
using System.Text;
using Vulild.Service.Attributes;

namespace Vulild.Service.AssemblyService
{
    [ServiceOption(Type = typeof(AssemblySearch))]
    public class AssemblySearchOption : Option
    {
        AssemblySearch assemblySearch;
        public List<string> Paths;

        public List<string> ExcludePaths;
        public override IService CreateService()
        {
            if (assemblySearch == null)
            {
                assemblySearch = new AssemblySearch
                {
                    Paths = this.Paths,
                    ExcludePaths = this.ExcludePaths
                };
            }
            return assemblySearch;
        }
    }
}
