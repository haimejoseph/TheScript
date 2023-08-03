using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MI24_TheScriptApp.Service
{
    public static class XmlParser
    {
        public class EventData
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public string Package { get; set; }
            public string Value { get; set; }
        }



        public class EventAction
        {
            public string Name { get; set; }
            public string Package { get; set; }
            public string Type { get; set; }
            public string Value { get; set; }
        }



        public class Event
        {
            public string Name { get; set; }
            public string Package { get; set; }
            public DateTime Timestamp { get; set; }
            public List<EventData> Data { get; set; }
            public List<EventAction> Actions { get; set; }
        }



        public class RingBufferTarget
        {
            public int Truncated { get; set; }
            public int ProcessingTime { get; set; }
            public int TotalEventsProcessed { get; set; }
            public int EventCount { get; set; }
            public int DroppedCount { get; set; }
            public int MemoryUsed { get; set; }
            public List<Event> Events { get; set; }
        }
        public static RingBufferTarget ParseXml(string xmlData)
        {
            var ringBufferTarget = new RingBufferTarget();
            ringBufferTarget.Events = new List<Event>();



            var xmlDoc = XDocument.Parse(xmlData);
            var ringBufferElement = xmlDoc.Element("RingBufferTarget");



            ringBufferTarget.Truncated = int.Parse(ringBufferElement.Attribute("truncated").Value);
            ringBufferTarget.ProcessingTime = int.Parse(ringBufferElement.Attribute("processingTime").Value);
            ringBufferTarget.TotalEventsProcessed = int.Parse(ringBufferElement.Attribute("totalEventsProcessed").Value);
            ringBufferTarget.EventCount = int.Parse(ringBufferElement.Attribute("eventCount").Value);
            ringBufferTarget.DroppedCount = int.Parse(ringBufferElement.Attribute("droppedCount").Value);
            ringBufferTarget.MemoryUsed = int.Parse(ringBufferElement.Attribute("memoryUsed").Value);



            foreach (var eventElement in ringBufferElement.Elements("event"))
            {
                var eventDataList = new List<EventData>();
                var eventActionList = new List<EventAction>();



                foreach (var dataElement in eventElement.Elements("data"))
                {
                    eventDataList.Add(new EventData
                    {
                        Name = dataElement.Attribute("name").Value,
                        Type = dataElement.Element("type").Attribute("name").Value,
                        Package = dataElement.Element("type").Attribute("package").Value,
                        Value = dataElement.Element("value").Value
                    });
                }



                foreach (var actionElement in eventElement.Elements("action"))
                {
                    eventActionList.Add(new EventAction
                    {
                        Name = actionElement.Attribute("name").Value,
                        Package = actionElement.Element("type").Attribute("package").Value,
                        Type = actionElement.Element("type").Attribute("name").Value,
                        Value = actionElement.Element("value").Value
                    });
                }



                ringBufferTarget.Events.Add(new Event
                {
                    Name = eventElement.Attribute("name").Value,
                    Package = eventElement.Attribute("package").Value,
                    Timestamp = DateTime.Parse(eventElement.Attribute("timestamp").Value),
                    Data = eventDataList,
                    Actions = eventActionList
                });
            }



            return ringBufferTarget;
        }
    }
}
