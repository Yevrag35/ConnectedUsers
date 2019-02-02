using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace MG.QUserModule.Objects
{
    public class QUserObject : IQUserObject
    {
        private const string GenericAttsGetMethod = "GetAttribute";

        public string HostName { get; }
        public string UserName { get; }
        public string SessionName { get; }
        //public bool IsCurrentSession { get; }
        public SessionState State { get; }
        public int Id { get; }
        public TimeSpan? IdleTime { get; }
        public DateTime LogonTime { get; }

        #region CONSTRUCTORS
        public QUserObject(string hn, string un, string sn, string state, int id, string it, string lot)
        {
            if (!string.IsNullOrWhiteSpace(it) && int.TryParse(it, out var tryint))
            {
                it = string.Format("0:{0}", tryint);
            }
            if (!string.IsNullOrWhiteSpace(it) && TimeSpan.TryParse(it, out var ts))
            {
                this.IdleTime = ts;
            }

            this.HostName = hn;
            this.UserName = un;
            this.SessionName = sn;
            //IsCurrentSession = isCur;
            this.State = this.GetSessionState(state);
            this.Id = id;
            this.LogonTime = DateTime.Parse(lot);
        }

        #endregion

        #region ENUM TRANSLATIONS
        private SessionState GetSessionState(string stateStr)
        {
            return this.GetEnumFromValue<SessionState>(stateStr, typeof(NameAttribute));
        }

        private T[] GetEnumValues<T>() where T : Enum
        {
            var arr = typeof(T).GetEnumValues();
            var tArr = new T[arr.Length];
            for (var i = 0; i < arr.Length; i++)
            {
                var val = (T)arr.GetValue(i);
                tArr[i] = val;
            }
            return tArr;
        }

        private T GetEnumFromValue<T>(object value, Type attributeType) where T : Enum
        {
            var arr = this.GetEnumValues<T>();
            for (var i = 0; i < arr.Length; i++)
            {
                var v = (Enum)arr.GetValue(i);
                var o = this.GetAttributeValue<object>(v, attributeType);
                if (o.Equals(value))
                {
                    return (T)v;
                }
            }
            return default;
        }

        private T GetAttributeValue<T>(Enum e, Type attributeType)
        {
            if (!attributeType.GetInterfaces().Contains(typeof(IAttribute)))
                throw new ArgumentException("This method only supports attributes who inherit 'IAttribute'.");
            else if (attributeType.IsInterface)
                throw new ArgumentException("This method does not support interfaces for the attributeType!");

            var castedObj = (IAttribute)this.InvokeGenericGetAtts(e, attributeType);
            if (castedObj.ValueIsArray && !castedObj.ValueIsOneItemArray)
                throw new InvalidOperationException("The casted object has multiple values!");

            else if (castedObj.ValueIsOneItemArray)
            {
                var objs = ((IEnumerable)castedObj.Value).Cast<object>().ToArray();
                return (T)objs[0];
            }

            else
                return (T)castedObj.Value;

        }

        private T GetAttribute<T>(FieldInfo fi) where T : Attribute, IAttribute =>
            fi.GetCustomAttribute<T>(false);

        private object InvokeGenericGetAtts(Enum e, Type attType)
        {
            var fi = this.GetFieldInfo(e);
            var t = this.GetType();

            var mi = t.GetMethod(GenericAttsGetMethod, BindingFlags.Instance | BindingFlags.NonPublic);
            var mgm = mi.MakeGenericMethod(attType);
            object outObj = null;
            try
            {
                outObj = mgm.Invoke(this, new object[1] { fi });
            }
            catch (TargetInvocationException)
            {
            }
            catch (Exception genEx)
            {
                throw new GenericMethodException(attType, GenericAttsGetMethod, e, genEx.InnerException);
            }
            return outObj;
        }

        private FieldInfo GetFieldInfo(Enum e) =>
            e.GetType().GetField(e.ToString());

        #endregion
    }
}
