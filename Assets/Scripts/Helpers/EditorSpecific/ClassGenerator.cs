using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace Utilities
{
    public class ClassGenerator
    {
        private StringBuilder sb = new StringBuilder();
        private StringBuilder mainSb = new StringBuilder();

        private int newMemberPositionIndex;
        private int newClassAttributePositionIndex;
        private int propertiesTabsCount;
        public ClassGenerator CreateClass(string className, string classNamespace = "",
            AccessModifier classAccessModifiers = AccessModifier.Public,
            ObjectType objectType = ObjectType.Class,
            IEnumerable<Type> attributes = null)
        {
            sb.Length = 0;
            mainSb.Length = 0;

            int tabs = 0;
            if (classNamespace != "")
            {
                sb.Append("namespace").Space().Append(classNamespace);
                tabs++;
                sb.OpenBracket(tabs);
            }
            if (attributes != null)
            {
                AppendAttributes(sb, attributes);
                newClassAttributePositionIndex = sb.Length;
                sb.NewTabbedLine(tabs);
            }
            AppendModifiers(sb, classAccessModifiers);
            sb.Append(objectType.ToString().ToLower());
            sb.Space();
            sb.Append(className);
            tabs++;
            sb.OpenBracket(tabs);

            newMemberPositionIndex = sb.Length;
            propertiesTabsCount = tabs;

            while (tabs > 0)
            {
                tabs--;
                sb.NewTabbedLine(tabs);
                sb.Append('}');
            }

            mainSb.Append(sb.ToString());
            return this;
        }

        public ClassGenerator CreateClass(string className, string classNamespace = "",
            AccessModifier classAccessModifiers = AccessModifier.Public,
            ObjectType objectType = ObjectType.Class,
            params Type[] attributes)
        {
            return CreateClass(className, classNamespace, classAccessModifiers, objectType,
                (IEnumerable<Type>)attributes);
        }

        public ClassGenerator AddClassAttribute(string attribute)
        {
            sb.Length = 0;
            sb.Append(attribute);
            InsertClassAttribute(sb.ToString());
            return this;
        }

        public ClassGenerator AddProperty(AccessModifier accessModifiers, Type propertyType, string propertyName,
            AdditionalModifiers additionalModifiers = AdditionalModifiers.None, string defaultValue = null,
            IEnumerable<Type> attributes = null)
        {
            sb.Length = 0;
            if (attributes != null)
            {
                AppendAttributes(sb, attributes);
            }
            AppendModifiers(sb, accessModifiers);
            AppendModifiers(sb, additionalModifiers);
            sb.Append(propertyType.FullName).Space();
            if (defaultValue != null)
            {
                sb.Append('=').Space().Append(defaultValue);
            }
            sb.Append(propertyName).Append(';').NewTabbedLine(propertiesTabsCount);

            InsertNewMember(sb.ToString());
            return this;
        }

        private void InsertClassAttribute(string value)
        {
            mainSb.Insert(newClassAttributePositionIndex, value);
            newClassAttributePositionIndex += value.Length;
            newMemberPositionIndex += value.Length;
        }
        private void InsertNewMember(string value)
        {
            mainSb.Insert(newMemberPositionIndex, value);
            newMemberPositionIndex += value.Length;
        }

       
        public ClassGenerator AddProperty(AccessModifier accessModifiers, Type propertyType, string propertyName,
            AdditionalModifiers additionalModifiers = AdditionalModifiers.None, string defaultValue = null,
            params Type[] attributes)
        {
            return AddProperty(
                accessModifiers, propertyType, propertyName, additionalModifiers, defaultValue,
                (IEnumerable<Type>)attributes);
        }

        public ClassGenerator AddMethod(AccessModifier accessModifiers, Type returnType, string name,
           IEnumerable<(Type paramType, string paramName)> parameters = null, params string[] methodCodeLines)
        {
            return AddMethod(accessModifiers, returnType, name, parameters, (IEnumerable<string>)methodCodeLines);
        }

        public ClassGenerator AddMethod(AccessModifier accessModifiers, Type returnType, string name,
           IEnumerable<(Type paramType, string paramName)> parameters = null,
           IEnumerable<string> methodCodeLines = null)
        {
            sb.Length = 0;
            sb.NewTabbedLine(propertiesTabsCount);
            AppendModifiers(sb, accessModifiers);
            string returnTypeName;
            if (returnType == typeof(void))
            {
                returnTypeName = "void";
            }
            else
            {
                returnTypeName = returnType.FullName;
            }
            sb.Append(returnTypeName).Space();
            sb.Append(name).Append('(');
            if (parameters != null)
            {
                AppendParameters(sb, parameters);
            }
            sb.Append(')');
            sb.OpenBracket(propertiesTabsCount + 1);
            foreach (var codeLine in methodCodeLines)
            {
                sb.Append(codeLine);
                sb.NewTabbedLine(propertiesTabsCount + 1);

            }
            sb.NewTabbedLine(propertiesTabsCount);
            sb.Append('}');
            InsertNewMember(sb.ToString());
            return this;
        }

        private void AppendParameters(StringBuilder sb, IEnumerable<(Type paramType, string paramName)> parameters)
        {
            int paramIndex = 0;
            int paramsCount = parameters.Count();
            foreach (var (parameterType, parameterName) in parameters)
            {
                if (paramIndex != 0)
                {
                    sb.Append(',').Space();
                }

                sb.Append(parameterType.FullName).Space();
                sb.Append(parameterName);

                paramIndex++;
            }
        }

        private void AppendModifiers(StringBuilder sb, Enum modifiersFlagEnum)
        {
            var flags = modifiersFlagEnum.GetFlags();
            foreach (var accessModifier in flags)
            {
                string accessModifierName = accessModifier.ToString().ToLower();
                sb.Append(accessModifierName);
                sb.Space();
            }
        }


        private void AppendAttributes(StringBuilder sb, IEnumerable<Type> attributes)
        {
            bool addedBraces = false;
            foreach (var attributeType in attributes)
            {

                if (typeof(Attribute).IsAssignableFrom(attributeType))
                {
                    if (addedBraces == false)
                    {
                        sb.Append('[');
                        addedBraces = true;
                    }
                    else
                    {
                        sb.Append(',').Space();
                    }
                    sb.Append(attributeType.FullName);
                }
            }
            if (addedBraces)
            {
                sb.Append(']').Space();
            }
        }
        public override string ToString()
        {
            return mainSb.ToString();
        }
        public void Log()
        {
            Debug.Log(mainSb);
        }

        [Flags]
        public enum AccessModifier
        {
            None = 0,
            Public = 1 << 0,
            Private = 1 << 1,
            Protected = 1 << 2,
            Internal = 1 << 3,
            Static = 1 << 4,
            Virtual = 1 << 5,
            Override = 1 << 6
        }

        [Flags]
        public enum AdditionalModifiers
        {
            None = 0,
            Ref = 1 << 0,
            ReadOnly = 1 << 1,
            Const = 1 << 2,
        }

        public enum ObjectType
        {
            Class = 0,
            Struct = 1,
        }

    }
}