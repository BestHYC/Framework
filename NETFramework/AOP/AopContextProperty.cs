using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace Framework
{
    public class AopContextProperty : IContextProperty, IContributeServerContextSink
    {
        public string Name { get; private set; } = nameof(AopContextProperty);

        public void Freeze(Context newContext)
        {
            //newContext.Freeze();
        }

        public IMessageSink GetServerContextSink(IMessageSink nextSink)
        {
            AopMessageSink sink = new AopMessageSink(nextSink);
            return sink;
        }

        public bool IsNewContextOK(Context newCtx)
        {
            IContextProperty property = newCtx.GetProperty(this.Name);
            if (property == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
