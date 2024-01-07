using Cocona.Application;
using Cocona.Command;
using Cocona.Command.BuiltIn;
using Cocona.Filters.Internal;
using Cocona.Help.DocumentModel;
using System.Text;
using Cocona.Localization.Internal;
using Cocona.Resources;

namespace Cocona.Help;

public class CoconaCommandHelpProvider : ICoconaCommandHelpProvider
{
    private readonly ICoconaApplicationMetadataProvider _applicationMetadataProvider;
    private readonly IServiceProvider _serviceProvider;
    private readonly CoconaLocalizerWrapper _localizer;

    public CoconaCommandHelpProvider(ICoconaApplicationMetadataProvider applicationMetadataProvider, IServiceProvider serviceProvider)
    {
        _applicationMetadataProvider = applicationMetadataProvider;
        _serviceProvider = serviceProvider;
        _localizer = new CoconaLocalizerWrapper(_serviceProvider);
    }

    public HelpMessage CreateCommandHelp(CommandDescriptor command, IReadOnlyList<CommandDescriptor> subCommandStack)
    {
        var help = new HelpMessage();

        // Usage
        help.Children.Add(new HelpSection(HelpSectionId.Usage, new HelpUsage(string.Format(Strings.Help_Index_Usage, _applicationMetadataProvider.BuildUsageCommandOptionsAndArgs(command, subCommandStack)))));

        // Description
        if (!string.IsNullOrWhiteSpace(command.Description))
        {
            help.Children.Add(new HelpSection(HelpSectionId.Description, new HelpDescription(_localizer.GetCommandDescription(command))));
        }

        // Arguments
        AddHelpForCommandArguments(help, command, command.Arguments);

        // Options
        AddHelpForCommandOptions(help, command, command.Options.OfType<ICommandOptionDescriptor>().Concat(command.OptionLikeCommands));

        // Transform help document
        var transformers = FilterHelper.GetFilters<ICoconaHelpTransformer>(command.Method, _serviceProvider);
        foreach (var transformer in transformers)
        {
            transformer.TransformHelp(help, command);
        }

        return help;
    }

    public HelpMessage CreateCommandsIndexHelp(CommandCollection commandCollection, IReadOnlyList<CommandDescriptor> subCommandStack)
    {
        var help = new HelpMessage();

        // Usage
        var usageSection = new HelpSection(HelpSectionId.Usage);
        var subCommandParams = (subCommandStack.Count > 0) ? string.Join(" ", subCommandStack.Select(x => x.Name)) + " " : "";
        if (commandCollection.All.Count != 1)
        {
            usageSection.Children.Add(new HelpUsage(string.Format(Strings.Help_Index_Usage_Multple, _applicationMetadataProvider.GetExecutableName(), subCommandParams)));
        }
        if (commandCollection.Primary != null && (commandCollection.All.Count == 1 || commandCollection.Primary.Options.Any() || commandCollection.Primary.Arguments.Any()))
        {
            usageSection.Children.Add(new HelpUsage(string.Format(Strings.Help_Index_Usage, _applicationMetadataProvider.BuildUsageCommandOptionsAndArgs(commandCollection.Primary, subCommandStack))));
        }
        help.Children.Add(usageSection);

        // Description
        var description = !string.IsNullOrWhiteSpace(commandCollection.Description)
            ? commandCollection.Description
            : !string.IsNullOrWhiteSpace(commandCollection.Primary?.Description)
                ? _localizer.GetCommandDescription(commandCollection.Primary!)
                : !string.IsNullOrWhiteSpace(_applicationMetadataProvider.GetDescription())
                    ? _applicationMetadataProvider.GetDescription()
                    : string.Empty;

        if (!string.IsNullOrWhiteSpace(description))
        {
            help.Children.Add(new HelpSection(HelpSectionId.Description, new HelpDescription(description!)));
        }

        // Commands
        var commandsExceptPrimary = commandCollection.All.Where(x => !x.IsPrimaryCommand && !x.IsHidden).ToArray();
        if (commandsExceptPrimary.Any())
        {
            help.Children.Add(new HelpSection(HelpSectionId.Commands,
                new HelpHeading(Strings.Help_Heading_Commands),
                new HelpSection(
                    new HelpLabelDescriptionList(
                        commandsExceptPrimary
                            .Select((x, i) =>
                                new HelpLabelDescriptionListItem(x.Name, _localizer.GetCommandDescription(x))
                            )
                            .ToArray()
                    )
                )
            ));
        }

        // Show helps for primary command.
        if (commandCollection.Primary != null)
        {
            // Arguments
            AddHelpForCommandArguments(help, commandCollection.Primary, commandCollection.Primary.Arguments);

            // Options
            AddHelpForCommandOptions(help, commandCollection.Primary, commandCollection.Primary.Options.OfType<ICommandOptionDescriptor>().Concat(commandCollection.Primary.OptionLikeCommands));
        }

        // Transform help document
        if (commandCollection.Primary != null)
        {
            var transformers = FilterHelper.GetFilters<ICoconaHelpTransformer>(commandCollection.Primary.Method, _serviceProvider);

            // TODO: This is ad-hoc workaround for default primary command.
            if (BuiltInPrimaryCommand.IsBuiltInCommand(commandCollection.Primary))
            {
                transformers = commandCollection.All
                    .Select(x => x.CommandType)
                    .Distinct()
                    .SelectMany(x => FilterHelper.GetFilters<ICoconaHelpTransformer>(x, _serviceProvider))
                    .ToArray();
            }

            foreach (var transformer in transformers)
            {
                transformer.TransformHelp(help, commandCollection.Primary);
            }
        }

        return help;
    }

    public HelpMessage CreateVersionHelp()
    {
        var prodName = _applicationMetadataProvider.GetProductName();
        var version = _applicationMetadataProvider.GetVersion();

        return new HelpMessage(new HelpSection(new HelpHeading($"{prodName} {version}")));
    }

    private void AddHelpForCommandArguments(HelpMessage help, CommandDescriptor command, IReadOnlyList<CommandArgumentDescriptor> arguments)
    {
        if (arguments.Any())
        {
            help.Children.Add(new HelpSection(HelpSectionId.Arguments,
                new HelpHeading(Strings.Help_Heading_Arguments),
                new HelpSection(
                    new HelpLabelDescriptionList(
                        arguments
                            .Select((x, i) =>
                                new HelpLabelDescriptionListItem(
                                    $"{i}: {x.Name}",
                                    CoconaLabelBuilder.BuildParameterDescription(_localizer.GetArgumentDescription(command, x), x.IsRequired, x.UnwrappedArgumentType, x.DefaultValue)
                                )
                            )
                            .ToArray()
                    )
                )
            ));
        }
    }

    private void AddHelpForCommandOptions(HelpMessage help, CommandDescriptor command, IEnumerable<ICommandOptionDescriptor> options)
    {
        if (options.Any(x => !x.Flags.HasFlag(CommandOptionFlags.Hidden)))
        {
            help.Children.Add(new HelpSection(HelpSectionId.Options,
                new HelpHeading(Strings.Help_Heading_Options),
                new HelpSection(
                    new HelpLabelDescriptionList(
                        options
                            .Where(x => !x.Flags.HasFlag(CommandOptionFlags.Hidden))
                            .Select((x, i) =>
                                x is CommandOptionDescriptor option
                                    ? new HelpLabelDescriptionListItem(
                                        option.BuildParameterLabel(),
                                        CoconaLabelBuilder.BuildParameterDescription(_localizer.GetOptionDescription(command, x), option.IsRequired, option.UnwrappedOptionType, option.DefaultValue)
                                    )
                                    : x is CommandOptionLikeCommandDescriptor optionLikeCommand
                                        ? new HelpLabelDescriptionListItem(
                                            optionLikeCommand.BuildParameterLabel(),
                                            _localizer.GetCommandDescription(optionLikeCommand.Command)
                                        )
                                        : throw new NotSupportedException()
                            )
                            .ToArray()
                    )
                )
            ));
        }
    }


}
