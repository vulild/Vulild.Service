using System;
using System.Collections.Generic;
using System.Text;

namespace Vulild.Service.RepoService
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRepoService
    {
        OUT GetRepo<IN, OUT>(IN param);
    }
}
