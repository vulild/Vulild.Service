using Vulild.Service.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Vulild.Core.FormatConversion;

namespace Vulild.Service
{
    public abstract class Option
    {
        public bool IsDefault { get; set; }

        private Dictionary<string, string> _MessageDataTypeMap;

        public Dictionary<string, string> MessageDataTypeMap
        {
            get
            {
                if (_MessageDataTypeMap == null)
                {
                    _MessageDataTypeMap = new Dictionary<string, string>();
                }
                return _MessageDataTypeMap;
            }
            set
            {
                _MessageDataTypeMap = value;
            }
        }

        ///// <summary>
        ///// 根据<paramref name="type"/>获取映射到mq的类型
        ///// </summary>
        ///// <param name="type"></param>
        ///// <returns></returns>
        //public string GetMapType(string type)
        //{
        //    var mapTypes = this.MessageDataTypeMap.Where(a => a.Value.ToType().IsAssignableFrom(type.ToType()));
        //    if (mapTypes.Any())
        //    {
        //        return mapTypes.FirstOrDefault().Key;
        //    }

        //    throw new TypeNotFoundException(type);
        //}

        public virtual IService Build()
        {
            var service = this.CreateService();
            service.Option = this;
            return service;
        }

        public abstract IService CreateService();

        public virtual void Init()
        {

        }

    }
}
