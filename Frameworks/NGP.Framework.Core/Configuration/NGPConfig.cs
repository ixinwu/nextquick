/* ---------------------------------------------------------------------    
 * Copyright:
 * IXinWu Technology Co., Ltd. All rights reserved. 
 * 
 * NGPConfig Description:
 * ϵͳ����
 *
 * Comment 					        Revision	Date        Author
 * -----------------------------    --------    --------    -----------
 * Created							1.0		    2019-1-15   hulei@ixinwu.com
 *
 * ------------------------------------------------------------------------------*/

namespace NGP.Framework.Core
{
    /// <summary>
    /// ����ʱ�����ò���
    /// </summary>
    public partial class NGPConfig
    {
        /// <summary>
        /// ��ȡ������һ��ֵ����ֵָʾ�Ƿ�Ӧʹ��Redis���������л��棨Ĭ�����ڴ滺�棩
        /// </summary>
        public bool RedisCachingEnabled { get; set; }
        /// <summary>
        /// ��ȡredis�������Ӵ�
        /// </summary>
        public string RedisCachingConnectionString { get; set; }

        /// <summary>
        /// ����Secret
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// token����ʱ�䣨��λСʱ��
        /// </summary>
        public string TokenExpiresHour { get; set; }

        /// <summary>
        /// �Ƿ�������־
        /// </summary>
        public bool LogEnabled { get; set; }
    }
}