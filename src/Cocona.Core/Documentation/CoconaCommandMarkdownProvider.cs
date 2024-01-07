using System.Text;
using Cocona.Application;
using Cocona.Command;
using Cocona.Help;
using Cocona.Localization.Internal;
using Cocona.Resources;

namespace Cocona.Documentation;
public class CoconaCommandMarkdownProvider : ICoconaCommandDocumentationProvider
{
    private readonly ICoconaApplicationMetadataProvider _applicationMetadataProvider;
    private readonly IServiceProvider _serviceProvider;
    private readonly CoconaLocalizerWrapper _localizer;
    private readonly HeadingLevel _headingLevel;

    public enum HeadingLevel { Heading1 = 1, Heading2 = 2, Heading3 = 3, Heading4 = 4, Heading5 = 5 };

    public CoconaCommandMarkdownProvider(ICoconaApplicationMetadataProvider applicationMetadataProvider, IServiceProvider serviceProvider, HeadingLevel headingLevel = HeadingLevel.Heading1)
    {
        _applicationMetadataProvider = applicationMetadataProvider;
        _serviceProvider = serviceProvider;
        _localizer = new CoconaLocalizerWrapper(_serviceProvider);
        _headingLevel = headingLevel;
    }

    public string CreateCommandDocumentation(CommandDescriptor command, IReadOnlyList<CommandDescriptor> subCommandStack, bool multipleCommands = false)
    {
        var sb = new StringBuilder();
        var headingBase = new string('#', (int)_headingLevel);
        var headingNext = new string('#', (int)_headingLevel + 1);

        if (command.Flags.HasFlag(CommandFlags.Primary) && multipleCommands)
            sb.AppendLine($"{headingBase} {command.Name} (Default)");
        else
            sb.AppendLine($"{headingBase} {command.Name}");
        sb.AppendLine();

        // Command description
        if (!string.IsNullOrWhiteSpace(command.Description))
        {
            sb.AppendLine($"{headingNext} {Strings.Markdown_Heading_CommandDescription}");
            sb.AppendLine(_localizer.GetCommandDescription(command));
        }

        // Usage / command strucutre
        sb.AppendLine($"{headingNext} {Strings.Markdown_Heading_Usage}");
        sb.AppendLine("```bash");
        sb.AppendLine(_applicationMetadataProvider.BuildUsageCommandOptionsAndArgs(command, subCommandStack));
        sb.AppendLine("```");

        // Arguments
        if (command.Arguments.Any())
        {
            sb.AppendLine($"{headingNext} {Strings.Markdown_Heading_Arguments}");
            sb.Append(AddHelpForCommandArguments(command, command.Arguments));
        }

        // Options
        if (command.Options.Where(i => !i.Flags.HasFlag(CommandOptionFlags.Hidden)).Any())
        {
            sb.AppendLine($"{headingNext} {Strings.Markdown_Heading_Options}");
            sb.Append(AddHelpForCommandOptions(command, command.Options.OfType<ICommandOptionDescriptor>().Concat(command.OptionLikeCommands)));
        }

        return sb.ToString();
    }

    private string BuildArgumentDescription(CommandDescriptor command, CommandArgumentDescriptor argument)
        => CoconaLabelBuilder.BuildParameterDescription(_localizer.GetArgumentDescription(command, argument), argument.IsRequired, argument.UnwrappedArgumentType, argument.DefaultValue);

    private string AddHelpForCommandArguments(CommandDescriptor command, IReadOnlyList<CommandArgumentDescriptor> arguments)
    {
        return string.Join(null,
            arguments.Select((argument, index) => $"{index + 1}. **{argument.Name}**: {BuildArgumentDescription(command, argument)}{Environment.NewLine}")
        );

    }

    private string AddHelpForCommandOptions(CommandDescriptor command, IEnumerable<ICommandOptionDescriptor> options)
    {
        var sb = new StringBuilder();
        foreach (var option in options)
        {
            if (option is null)
                continue;

            if (option.Flags.HasFlag(CommandOptionFlags.Hidden))
                continue;

            var type = option.GetType().Name;
            sb.AppendLine(type switch
            {
                nameof(CommandOptionDescriptor) => $"- {((CommandOptionDescriptor)option).BuildParameterLabel()}: {CoconaLabelBuilder.BuildParameterDescription(_localizer.GetOptionDescription(command, (CommandOptionDescriptor)option), ((CommandOptionDescriptor)option).IsRequired, ((CommandOptionDescriptor)option).UnwrappedOptionType, ((CommandOptionDescriptor)option).DefaultValue)}",
                nameof(CommandOptionLikeCommandDescriptor) => $"- {((CommandOptionLikeCommandDescriptor)option).BuildParameterLabel()}: {_localizer.GetCommandDescription(((CommandOptionLikeCommandDescriptor)option).Command)}",
                _ => throw new NotSupportedException()
            });
        }
        return sb.ToString();
    }

    public string CreateCommandsDocumentation(CommandCollection commandCollection, IReadOnlyList<CommandDescriptor> commandDescriptors)
    {
        var sb = new StringBuilder();
        var headingBase = new string('#', (int)_headingLevel);

        // application description
        if (!string.IsNullOrWhiteSpace(_applicationMetadataProvider.GetDescription()))
        {
            sb.AppendLine($"{headingBase} {Strings.Markdown_Heading_ApplicationDescription}");
            sb.AppendLine(_applicationMetadataProvider.GetDescription());
        }
        foreach (var command in commandCollection.All)
        {
            sb.Append(CreateCommandDocumentation(command, commandDescriptors, commandCollection.All.Count > 1));
        }
        return sb.ToString();
    }
}

