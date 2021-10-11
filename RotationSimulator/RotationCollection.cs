using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace RotationSimulator
{
    public class RotationCollection : SortedDictionary<string, Rotation>, IXmlSerializable
    {
        public XmlSchema GetSchema() {
            return null;
        }

        public void ReadXml(XmlReader reader) {
            if (reader.IsEmptyElement) { return; }

            reader.Read(); //Rotations Node
            reader.Read(); //Rotation Node
            while (reader.NodeType != XmlNodeType.EndElement && reader.Name == "Rotation") {
                Rotation r = new Rotation()
                {
                    DisplayName = reader.GetAttribute("DisplayName"),
                    JobCode = reader.GetAttribute("JobCode"),
                    StartTimeOffset = Int32.Parse(reader.GetAttribute("StartTimeOffset"))
                };

                reader.Read();//RotationSteps
                reader.Read();//RotationStep
                if (reader.Name == "RotationStep") {
                    while (reader.NodeType != XmlNodeType.EndElement && reader.Name == "RotationStep") {
                        ERotationStepType eRotationStepType = Enum.Parse<ERotationStepType>(reader.GetAttribute("Type"));
                        RotationStep step = new RotationStep()
                        {
                            Type = eRotationStepType
                        };
                        reader.Read(); //RotationParams
                        step.Parameters.ReadXml(reader); //will read off the RotationParams tag
                        r.RotationSteps.Add(step);
                        reader.Read();// </RotationStep
                    }
                    reader.Read();// </RotationSteps
                }
                reader.Read();// </Rotation
                Add(r.DisplayName,r);
            }
            reader.Read();
        }

        public void WriteXml(XmlWriter writer) {
            writer.WriteStartElement("Rotations");
            foreach (Rotation r in Values) {
                writer.WriteStartElement("Rotation");
                {
                    writer.WriteAttributeString("DisplayName", r.DisplayName);
                    writer.WriteAttributeString("JobCode", r.JobCode);
                    writer.WriteAttributeString("StartTimeOffset", r.StartTimeOffset.ToString());
                    writer.WriteStartElement("RotationSteps");
                    {
                        foreach (RotationStep step in r.RotationSteps) {
                            writer.WriteStartElement("RotationStep");
                            writer.WriteAttributeString("Type", step.Type.ToString());
                            writer.WriteStartElement("RotationParams");
                            step.Parameters.WriteXml(writer);
                            writer.WriteEndElement();
                            writer.WriteEndElement();
                        }
                    }
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
    }
}
