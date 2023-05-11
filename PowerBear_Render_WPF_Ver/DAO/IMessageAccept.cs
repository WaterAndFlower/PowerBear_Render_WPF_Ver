using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBear_Render_WPF_Ver.DAO {
    public enum AcceptType { DoneRender, NeedRenderTag }
    interface IMessageAccept { // 消息通知函数，通过这个来注册消息
        public abstract void AcceptFunc(AcceptType acceptType, Object sendData); // 用于接受消息
    }
}
