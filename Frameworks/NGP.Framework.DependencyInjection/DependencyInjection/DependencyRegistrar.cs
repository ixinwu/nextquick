/* ---------------------------------------------------------------------    
 * Copyright:
 * IXinWu Technology Co., Ltd. All rights reserved. 
 * 
 * DependencyRegistrar Description:
 * ����ע��
 *
 * Comment 					        Revision	Date        Author
 * -----------------------------    --------    --------    -----------
 * Created							1.0		    2019-1-20   hulei@ixinwu.com
 *
 * ------------------------------------------------------------------------------*/

using Autofac;
using Autofac.Configuration;
using Autofac.Extras.DynamicProxy;
using Microsoft.Extensions.Configuration;
using NGP.Framework.Core;
using System.Linq;

namespace NGP.Framework.DependencyInjection
{
    /// <summary>
    /// ����ע��
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {
        /// <summary>
        /// ע�����ͽӿ�
        /// </summary>
        /// <param name="builder">Container builder</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="config">Config</param>
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            var config = new ConfigurationBuilder();
            config.AddJsonFile("autofac.json");

            // Register the ConfigurationModule with Autofac.
            var module = new ConfigurationModule(config.Build());
            builder.RegisterModule(module);

            // ���������
            var assemblies = typeFinder.GetAssemblies().Where(s => s.FullName.StartsWith("NGP.Foundation.")).ToArray();
            
            builder.RegisterType<ExceptionInterceptionHandler>().InstancePerLifetimeScope();
            builder.RegisterType<TransactionInterceptorHandler>().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(assemblies)
                .Where(s => s.Name.EndsWith("Service"))
                .AsImplementedInterfaces()
                .EnableInterfaceInterceptors()
                .InterceptedBy(typeof(ExceptionInterceptionHandler), typeof(TransactionInterceptorHandler));
        }

        /// <summary>
        /// ע��˳��
        /// </summary>
        public int Order
        {
            get { return 2; }
        }
    }
}
