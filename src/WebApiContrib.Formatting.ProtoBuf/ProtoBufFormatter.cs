using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using ProtoBuf;
using ProtoBuf.Meta;

namespace WebApiContrib.Formatting
{
    public class ProtoBufFormatter : MediaTypeFormatter
    {
        private static readonly MediaTypeHeaderValue mediaType = new MediaTypeHeaderValue("application/x-protobuf");
        private static Lazy<RuntimeTypeModel> model = new Lazy<RuntimeTypeModel>(CreateTypeModel);

        public static RuntimeTypeModel Model
        {
            get { return model.Value; }
        }

        public ProtoBufFormatter()
        {
            SupportedMediaTypes.Add(mediaType);
        }

        public static MediaTypeHeaderValue DefaultMediaType
        {
            get { return mediaType; }
        }

    	public override bool CanReadType(Type type)
        {
            return CanReadTypeCore(type);
        }

    	public override bool CanWriteType(Type type)
        {
            return CanReadTypeCore(type);
        }

    	public override Task<object> ReadFromStreamAsync(Type type, Stream stream, HttpContent content, IFormatterLogger formatterLogger)
        {
            var tcs = new TaskCompletionSource<object>();

            try
            {
                object result = Model.Deserialize(stream, null, type);
                tcs.SetResult(result);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }

            return tcs.Task;
        }

    	public override Task WriteToStreamAsync(Type type, object value, Stream stream, HttpContent content, TransportContext transportContext)
        {
            var tcs = new TaskCompletionSource<object>();

            try
            {
                Model.Serialize(stream, value);
                tcs.SetResult(null);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }

            return tcs.Task;
        }

        private static RuntimeTypeModel CreateTypeModel()
        {
            var typeModel = TypeModel.Create();
            typeModel.UseImplicitZeroDefaults = false;
            return typeModel;
        }

        private static bool CanReadTypeCore(Type type)
        {
            return type.GetCustomAttributes(typeof(ProtoContractAttribute)).Any();
        }
    }
}
