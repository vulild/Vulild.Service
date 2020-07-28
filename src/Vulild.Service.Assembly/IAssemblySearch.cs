using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Vulild.Service.AssemblyService
{
    public delegate void AssemblySearchTypeDeal(Type type);

    public delegate void AssemblySearchAssmblyDeal(Assembly assembly);

    public interface IAssemblySearch : IService
    {
        event AssemblySearchAssmblyDeal AssemblyDeal;

        event AssemblySearchTypeDeal TypeDeal;

        void Search();
    }
}
