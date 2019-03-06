/* ---------------------------------------------------------------------    
 * Copyright:
 * IXinWu Technology Co., Ltd. All rights reserved. 
 * 
 * RabbitMQPublisher Description:
 * RabbitMq��Ϣ������
 *
 * Comment 					        Revision	Date        Author
 * -----------------------------    --------    --------    -----------
 * Created							1.0		    2019/3/4 11:20:22 hulei@ixinwu.com
 *
 * ------------------------------------------------------------------------------*/
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NGP.Framework.Core;

namespace NGP.Middleware.MessageQueue
{
    /// <summary>
    /// RabbitMq��Ϣ������
    /// </summary>
    public class RabbitMQPublisher : IMessagePublisher
    {
        /// <summary>
        /// ģʽ
        /// </summary>
        private static readonly Dictionary<string, IModel> model = new Dictionary<string, IModel>();

        /// <summary>
        /// ��������
        /// </summary>
        /// <typeparam name="T">�������ݶ���</typeparam>
        /// <param name="mqInfo">Ŀ��ͨ����Ϣ</param>
        /// <param name="data">����</param>
        public void Send<T>(MessageRouteInfo mqInfo, T data)
        {
            try
            {
                // ��������
                var connectionFactory = new ConnectionFactory();
                connectionFactory.HostName = mqInfo.HostName;
                connectionFactory.UserName = mqInfo.UserName;
                connectionFactory.Password = mqInfo.Password;
                connectionFactory.AutomaticRecoveryEnabled = true;
                using (var connection = connectionFactory.CreateConnection())
                {
                    using (var model = connection.CreateModel())
                    {
                        // ע�ύ����
                        model.ExchangeDeclare(mqInfo.ExchangeName, ExchangeType.Fanout, true);

                        // ע�����
                        model.QueueDeclare(mqInfo.QueueName, mqInfo.QueueDurable, false, false, null);

                        // ����ת�����ֽ���
                        var json = JsonConvert.SerializeObject(data, new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.All
                        });
                        var messageBodyBytes = Encoding.UTF8.GetBytes(json);

                        // ��������
                        model.BasicPublish(mqInfo.ExchangeName,
                            mqInfo.RouteKey,
                            null, 
                            messageBodyBytes);
                    }
                }
            }
            catch(Exception ex)
            {
                var type = this.GetType();
                // д��־
                var info = new ErrorLogInfo
                {
                    BusinessMethod = string.Format("{0}.{1}", type.FullName, "Excute"),
                    ExceptionContent = string.IsNullOrEmpty(ex.Message) ? (ex.InnerException != null ? ex.InnerException.Message : "Unknow Error") : ex.Message,
                    Exception = ex
                };
                Singleton<IEngine>.Instance.Resolve<ILogPublisher>().RegisterError(info);
            }
        }
    }
}
