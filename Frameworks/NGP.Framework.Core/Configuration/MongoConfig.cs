/* ---------------------------------------------------------------------    
 * Copyright:
 * IXinWu Technology Co., Ltd. All rights reserved. 
 * 
 * MongoConfig Description:
 * mongo����
 *
 * Comment 					        Revision	Date        Author
 * -----------------------------    --------    --------    -----------
 * Created							1.0		    2019-5-30   hulei@ixinwu.com
 *
 * ------------------------------------------------------------------------------*/

namespace NGP.Framework.Core
{
    /// <summary>
    /// mongo����
    /// </summary>
    public partial class MongoConfig
    {
        /// <summary>
        /// ���Ӵ�
        /// </summary>
        public string Connection { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        public string DatabaseName { get; set; }
    }
}