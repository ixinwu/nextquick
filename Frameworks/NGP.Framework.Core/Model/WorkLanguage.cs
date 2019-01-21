/* ---------------------------------------------------------------------    
 * Copyright:
 * IXinWu Technology Co., Ltd. All rights reserved. 
 * 
 * WorkLanguage Description:
 * ��������
 *
 * Comment 					        Revision	Date        Author
 * -----------------------------    --------    --------    -----------
 * Created							1.0		    2019-1-15   hulei@ixinwu.com
 *
 * ------------------------------------------------------------------------------*/

namespace NGP.Framework.Core
{
    /// <summary>
    /// ��������
    /// </summary>
    public partial class WorkLanguage : BaseEntity
    {
        /// <summary>
        /// ����
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// �����Ļ�
        /// </summary>
        public string LanguageCulture { get; set; }

        /// <summary>
        /// �����ļ�
        /// </summary>
        public string FlagImageFileName { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        public int DisplayOrder { get; set; }
    }
}
