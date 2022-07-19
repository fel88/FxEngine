using System;

namespace FxEngine
{
    public class ClickProcessorAttribute : Attribute
    {
        public string Id { get; set; }
    }

    public class TextChangedProcessorAttribute : Attribute
    {
        public string Id { get; set; }
    }
}
