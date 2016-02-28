/// <summary>
///  Enumerates the different s_unit types for translation and text generation.
///  See Sridhara, et. al (2010) section 3.4 Text Generation, paragraph 5.
/// </summary>

namespace Swummary
{
    public enum SUnitType
    {
        SingleMethodCall,
        NestedMethodCall,
        ComposedMethodCall,
        Assignment,
        Return,
        Conditional,
        Loop,
    }
}