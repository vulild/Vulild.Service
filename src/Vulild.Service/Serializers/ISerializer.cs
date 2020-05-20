using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vulild.Service.Serializers
{
    public interface ISerializer<T>
    {
        string Serialize(T t);

        T Deserialize(string pams);
    }
}
