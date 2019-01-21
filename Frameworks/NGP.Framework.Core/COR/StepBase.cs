/* ---------------------------------------------------------------------    
 * Copyright:
 * IXinWu Technology Co., Ltd. All rights reserved. 
 * 
 * IStep Description:
 * ְ����ģʽ��ÿ��ְ�𣨴����裩��Ҫ�̳еĻ���
 *
 * Comment 					        Revision	Date        Author
 * -----------------------------    --------    --------    -----------
 * Created							1.0		    2019-1-15   hulei@ixinwu.com
 *
 * ------------------------------------------------------------------------------*/


using System;
using System.Collections.Generic;

namespace NGP.Framework.Core
{
    /// <summary>
    /// ְ����ģʽ��ÿ��ְ�𣨴����裩��Ҫ�̳еĻ���
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public abstract class StepBase<TContext> : IStep<TContext>
    {
        private readonly List<IStep<TContext>> _trueSteps = new List<IStep<TContext>>();

        private readonly List<IStep<TContext>> _falseSteps = new List<IStep<TContext>>();
        /// <summary>
        /// �����һ����
        /// </summary>
        /// <param name="step">��һ����</param>
        /// <param name="resultFor">�������һ����������һ���ɹ���ʧ�ܺ�ִ��</param>
        /// <returns>��һ����</returns>
        public IStep<TContext> AddNextStep(IStep<TContext> step, bool resultFor = true)
        {
            if (resultFor)
            {
                _trueSteps.Add(step);
            }
            else
            {
                _falseSteps.Add(step);
            }
            return step;
        }
        /// <summary>
        /// ���账������ص�
        /// </summary>
        public EventHandler<Tuple<TContext, bool>> OnStepComplete { get; set; }
        /// <summary>
        /// ��ȡ��һ�����б�
        /// </summary>
        /// <param name="resultFor"></param>
        /// <returns></returns>
        public List<IStep<TContext>> GetNextSteps(bool resultFor)
        {
            return resultFor ? _trueSteps : _falseSteps;
        }

        /// <summary>
        /// ִ��������
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public abstract bool Process(TContext ctx);
        /// <summary>
        /// �������
        /// </summary>
        /// <param name="ctx">����������</param>
        public virtual void HandleProcess(TContext ctx)
        {
            bool result = Process(ctx);

            OnStepComplete?.Invoke(this, new Tuple<TContext, bool>(ctx, result));

            var nextSteps = GetNextSteps(result);

            if (nextSteps != null && nextSteps.Count > 0)
            {
                foreach (IStep<TContext> step in nextSteps)
                {
                    step.HandleProcess(ctx);
                }
            }
        }
    }
}
