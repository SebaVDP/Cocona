using Cocona.Command;

namespace Cocona.Documentation;

public interface ICoconaCommandDocumentationProvider
{
    string CreateCommandDocumentation(CommandDescriptor command, IReadOnlyList<CommandDescriptor> subCommandStack, bool multipleCommands = false);
    string CreateCommandsDocumentation(CommandCollection commandCollection, IReadOnlyList<CommandDescriptor> commandDescriptors);
}

