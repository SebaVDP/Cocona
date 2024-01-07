using System.Diagnostics.Contracts;
using System.Text;
using Cocona.Application;
using Cocona.Command;
using Cocona.Resources;

namespace Cocona.Help;

public static class CoconaLabelBuilder
{
    public static string BuildUsageCommandOptionsAndArgs(this ICoconaApplicationMetadataProvider applicationMetadataProvider, CommandDescriptor command, IReadOnlyList<CommandDescriptor> subCommandStack)
    {
        var sb = new StringBuilder();
        sb.Append(applicationMetadataProvider.GetExecutableName());

        if (subCommandStack.Count > 0)
        {
            sb.Append(" ");
            sb.Append(string.Join(" ", subCommandStack.Select(x => x.Name)));
        }

        if (!command.IsPrimaryCommand)
        {
            sb.Append(" ");
            sb.Append(command.Name);
        }

        if (command.Options.Any(x => !x.IsHidden))
        {
            foreach (var opt in command.Options.Where(x => !x.IsHidden))
            {
                sb.Append(" ");
                if (opt.UnwrappedOptionType == typeof(bool))
                {
                    if (opt.DefaultValue.HasValue && opt.DefaultValue.Value != null && opt.DefaultValue.Value.Equals(true))
                    {
                        sb.Append($"[--{opt.Name}=<true|false>]");
                    }
                    else
                    {
                        sb.Append($"[--{opt.Name}]");
                    }
                }
                else if (opt.IsEnumerableLike)
                {
                    sb.Append($"[--{opt.Name} <{opt.ValueName}>...]");
                }
                else
                {
                    sb.Append($"[--{opt.Name} <{opt.ValueName}>]");
                }
            }
        }

        if (command.OptionLikeCommands.Any(x => !x.IsHidden))
        {
            foreach (var opt in command.OptionLikeCommands.Where(x => !x.IsHidden))
            {
                sb.Append(" ");
                sb.Append($"[--{opt.Name}]");
            }
        }

        if (command.Arguments.Any())
        {
            foreach (var arg in command.Arguments)
            {
                sb.Append(" ");
                if (arg.IsEnumerableLike)
                {
                    sb.Append($"{arg.Name}0 ... {arg.Name}N");
                }
                else
                {
                    sb.Append(arg.Name);
                }
            }
        }

        return sb.ToString();
    }

    [Pure]
    public static string BuildParameterLabel(this CommandOptionDescriptor option)
    {
        return (option.ShortName.Any() ? string.Join(", ", option.ShortName.Select(x => $"-{x}")) + ", " : "") +
               $"--{option.Name}" +
               (
                   option.UnwrappedOptionType == typeof(bool)
                       ? option.DefaultValue.HasValue && option.DefaultValue.Value != null && option.DefaultValue.Value.Equals(true)
                           ? "=<true|false>"
                           : ""
                       : option.IsEnumerableLike
                           ? $" <{option.ValueName}>..."
                           : $" <{option.ValueName}>"
               );
    }

    [Pure]
    public static string BuildParameterLabel(this CommandOptionLikeCommandDescriptor optionLikeCommand)
    {
        return (optionLikeCommand.ShortName.Any() ? string.Join(", ", optionLikeCommand.ShortName.Select(x => $"-{x}")) + ", " : "") +
               $"--{optionLikeCommand.Name}";
    }

    [Pure]
    public static string BuildParameterDescription(string description, bool isRequired, Type valueType, CoconaDefaultValue defaultValue)
    {
        return
            description +
            (isRequired
                ? string.Format(" ({0})", Strings.Help_Description_Required)
                : (valueType == typeof(bool) && defaultValue.Value != null && defaultValue.Value.Equals(false))
                    ? ""
                    : (defaultValue.Value is null || (defaultValue.Value is string defaultValueStr && string.IsNullOrEmpty(defaultValueStr)))
                        ? ""
                        : (" " + string.Format("({0}: {1})", Strings.Help_Description_Default, defaultValue.Value))) +
            (valueType.IsEnum
                ? " " + string.Format("({0}: {1})", Strings.Help_Description_AllowedValues, string.Join(", ", Enum.GetNames(valueType)))
                : "");
    }
}
