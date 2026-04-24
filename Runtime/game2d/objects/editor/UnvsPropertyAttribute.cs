using System;

namespace unvs.game2d.objects.editor
{
    [AttributeUsage(AttributeTargets.Field)]
    public class UnvsPropertyAttribute : Attribute
    {
       
        public UnvsPropertyTypeEnum PType { get; }
        public UnvsPropertyAttribute(UnvsPropertyTypeEnum pType)
        {
            PType = pType;
        }
    }
}