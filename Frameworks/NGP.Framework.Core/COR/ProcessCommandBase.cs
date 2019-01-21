/* ---------------------------------------------------------------------    
 * Copyright:
 * IXinWu Technology Co., Ltd. All rights reserved. 
 * 
 * IStep Description:
 * �����������--ÿ�����������������¸����������
 *
 * Comment 					        Revision	Date        Author
 * -----------------------------    --------    --------    -----------
 * Created							1.0		    2019-1-15   hulei@ixinwu.com
 *
 * ------------------------------------------------------------------------------*/


using System.Collections.Concurrent;

namespace NGP.Framework.Core
{
    /// <summary>
    /// �����������--ÿ�����������������¸����������
    /// </summary>

    public abstract class ProcessCommandBase<TContext> : IProcessCommand<TContext>
    {
        /// <summary>
        /// �̰߳�ȫ�Ĳ���
        /// </summary>
        private readonly ConcurrentQueue<IProcessCommand<TContext>> _steps = new ConcurrentQueue<IProcessCommand<TContext>>();

        /// <summary>
        /// �����һ����
        /// </summary>
        /// <param name="step">��һ����</param>
        public void AddNextStep(IProcessCommand<TContext> step)
        {
            // ���Ԫ��
            _steps.Enqueue(step);
        }

        /// <summary>
        /// ִ��������
        /// </summary>
        /// <param name="request">�������</param>
        /// <param name="ctx">�����Ĳ���</param>
        /// <returns>ִ�н��</returns>
        public abstract INGPResponse Process(INGPRequest request, TContext ctx);

        /// <summary>
        /// �������
        /// </summary>
        /// <param name="request">�������</param>
        /// <param name="ctx">�����Ĳ���</param>
        /// <returns>���ش�����</returns>
        public virtual INGPResponse HandleProcess(INGPRequest request, TContext ctx)
        {
            INGPResponse result = null;
            do
            {
                // ��ǰִ�н��
                result = Process(request, ctx);

                IProcessCommand<TContext> step = null;
                if (!_steps.TryDequeue(out step) || step == null)
                {
                    break;
                }

                var nextRequest = result as INGPRequest;
                step.HandleProcess(nextRequest, ctx);
            }
            while (_steps.Count > 0);

            return result;
        }
    }
}
