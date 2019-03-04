/* ---------------------------------------------------------------------    
 * Copyright:
 * IXinWu Technology Co., Ltd. All rights reserved. 
 * 
 * NGPFileProvider Description:
 * NGP�ļ��ṩ��(IO����ʹ�ô����ϵ��ļ�ϵͳ)
 *
 * Comment 					        Revision	Date        Author
 * -----------------------------    --------    --------    -----------
 * Created							1.0		    2019-1-28   hulei@ixinwu.com
 *
 * ------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using NGP.Framework.Core;

namespace NGP.Framework.WebApi.Core
{
    /// <summary>
    /// NGP�ļ��ṩ��(IO����ʹ�ô����ϵ��ļ�ϵͳ)
    /// </summary>
    public class NGPWebFileProvider : NGPFileProvider
    {
        /// <summary>
        /// ��ʼ��NGPFileProvider����ʵ��
        /// </summary>
        /// <param name="hostingEnvironment">�йܻ���</param>
        public NGPWebFileProvider(IHostingEnvironment hostingEnvironment) 
            : base(File.Exists(hostingEnvironment.WebRootPath) ? Path.GetDirectoryName(hostingEnvironment.WebRootPath) : hostingEnvironment.ContentRootPath)
        {
        }
    }
}