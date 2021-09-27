using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace RotationSimulator
{
    public enum ERotationStepType
    {
        /// <summary>
        /// Parameters:
        ///     "action" - UniqueId of the ActionDef to execute.
        /// </summary>
        [EnumMember(Value ="action")]
        Action,
        /// <summary>
        /// Parameters:
        ///     "time" - Amount of time to wait.
        /// </summary>
        [EnumMember(Value = "wait")]
        Wait,
        [EnumMember(Value = "action_conditional")]
        ActionConditional,
        [EnumMember(Value = "bookmark")]
        Bookmark,
        [EnumMember(Value = "jump")]
        Jump
    }

    public class RotationStep
    {
        private static int IdCounter = 0;
        /// <summary>
        /// An arbitrary ID only valid for a given run of the program, but is nonetheless unique (barring someone making literally billions of elements in one run).
        /// </summary>
        public int Id { get; } = IdCounter++;

        public ERotationStepType Type { get; init; } = ERotationStepType.Action;
        public RotationStepParameters Parameters { get; set; } = new RotationStepParameters();

        public class RotationStepParameters : Dictionary<string, string>, IXmlSerializable
        {
            public XmlSchema GetSchema() {
                return null;
            }

            public void ReadXml(XmlReader reader) {
                if (reader.IsEmptyElement) { return; }

                reader.Read();
                while (reader.NodeType != XmlNodeType.EndElement) {
                    string key = reader.GetAttribute("Key");
                    string value = reader.GetAttribute("Value");

                    Add(key, value);

                    reader.Read();
                }
                reader.Read();
            }

            public void WriteXml(XmlWriter writer) {
                foreach (string key in Keys) {
                    writer.WriteStartElement("RotationParameter");
                    writer.WriteAttributeString("Key", key);
                    writer.WriteAttributeString("Value", this[key]);
                    writer.WriteEndElement();
                }
            }
        }
    }
}
