using Cocona.Application;
using Cocona.Command;
using Cocona.Documentation;
using Microsoft.Extensions.DependencyInjection;

namespace Cocona.Test.Help;

public class CoconaCommandMarkdownProviderTest
{
    private void __Dummy() { }

    class FakeApplicationMetadataProvider : ICoconaApplicationMetadataProvider
    {
        public string ProductName { get; set; } = "ProductName";
        public string Description { get; set; } = string.Empty;

        public string GetDescription() => Description;

        public string GetExecutableName() => "ExeName";

        public string GetProductName() => ProductName;

        public string GetVersion() => "1.0.0.0";
    }

    private CommandDescriptor CreateCommand(string name, string description, ICommandParameterDescriptor[] parameterDescriptors, CommandFlags flags = CommandFlags.None)
    {
        return new CommandDescriptor(
            typeof(CoconaCommandMarkdownProviderTest).GetMethod(nameof(CoconaCommandMarkdownProviderTest.__Dummy), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance),
            default,
            name,
            Array.Empty<string>(),
            description,
            Array.Empty<object>(),
            parameterDescriptors,
            parameterDescriptors.OfType<CommandOptionDescriptor>().ToArray(),
            parameterDescriptors.OfType<CommandArgumentDescriptor>().ToArray(),
            Array.Empty<CommandOverloadDescriptor>(),
            Array.Empty<CommandOptionLikeCommandDescriptor>(),
            flags,
            null
        );
    }

    private CommandOptionDescriptor CreateCommandOption(Type optionType, string name, IReadOnlyList<char> shortName, string description, CoconaDefaultValue defaultValue, CommandOptionFlags flags = CommandOptionFlags.None)
    {
        return new CommandOptionDescriptor(optionType, name, shortName, description, defaultValue, null, flags, Array.Empty<Attribute>());
    }

    [Fact]
    public void CommandHelp1()
    {
        // void Test(string arg0, string arg1, string arg2);
        // Arguments: new [] { "argValue0", "argValue1", "argValue2" }
        var commandDescriptor = CreateCommand(
            "Test",
            "command description",
            new ICommandParameterDescriptor[]
            {
                CreateCommandOption(typeof(string), "option0", Array.Empty<char>(), "option description - option0", CoconaDefaultValue.None),
                CreateCommandOption(typeof(bool), "option1", Array.Empty<char>(), "option description - option1", CoconaDefaultValue.None),
                new CommandIgnoredParameterDescriptor(typeof(bool), "ignored0", true),
                new CommandServiceParameterDescriptor(typeof(bool), "fromService0"),
                new CommandArgumentDescriptor(typeof(string), "arg0", 0, "description - arg0", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                new CommandArgumentDescriptor(typeof(string), "arg1", 1, "description - arg1", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                new CommandArgumentDescriptor(typeof(string), "arg2", 2, "description - arg2", CoconaDefaultValue.None, Array.Empty<Attribute>()),
            }
        );

        var provider = new CoconaCommandMarkdownProvider(new FakeApplicationMetadataProvider(), new ServiceCollection().BuildServiceProvider());
        var text = provider.CreateCommandDocumentation(commandDescriptor, Array.Empty<CommandDescriptor>());
    }

    [Fact]
    public void CommandsHelp_Single_Rendered()
    {
        var commandDescriptor = CreateCommand(
            "Test",
            "command description",
            new ICommandParameterDescriptor[]
            {
                CreateCommandOption(typeof(string), "foo", new [] { 'f' }, "Foo option", CoconaDefaultValue.None),
                CreateCommandOption(typeof(bool), "looooooong-option", new [] { 'l' }, "Long name option", new CoconaDefaultValue(false)),
            },
            CommandFlags.Primary
        );

        var provider = new CoconaCommandMarkdownProvider(new FakeApplicationMetadataProvider(), new ServiceCollection().BuildServiceProvider());
        var text = provider.CreateCommandsDocumentation(new CommandCollection(new[] { commandDescriptor }), Array.Empty<CommandDescriptor>());
        
        text.Should().Be(@"
# Test

## Description
command description
## Command structure
```bash
ExeName [--foo <String>] [--looooooong-option]
```
## Options
- -f, --foo <String>: Foo option (Required)
- -l, --looooooong-option: Long name option
".TrimStart());
    }

    [Fact]
    public void CommandsHelp_Single_NoDescription_Rendered()
    {
        var commandDescriptor = CreateCommand(
            "Test",
            "",
            new ICommandParameterDescriptor[]
            {
                CreateCommandOption(typeof(string), "foo", new [] { 'f' }, "Foo option", CoconaDefaultValue.None),
                CreateCommandOption(typeof(bool), "looooooong-option", new [] { 'l' }, "Long name option", new CoconaDefaultValue(false)),
            },
            CommandFlags.Primary
        );

        var provider = new CoconaCommandMarkdownProvider(new FakeApplicationMetadataProvider(), new ServiceCollection().BuildServiceProvider());
        var text = provider.CreateCommandsDocumentation(new CommandCollection(new[] { commandDescriptor }), Array.Empty<CommandDescriptor>());
        
        text.Should().Be(@"
# Test

## Command structure
```bash
ExeName [--foo <String>] [--looooooong-option]
```
## Options
- -f, --foo <String>: Foo option (Required)
- -l, --looooooong-option: Long name option
".TrimStart());
    }

    [Fact]
    public void CommandsHelp_Single_DescriptionFromMetadata_Rendered()
    {
        var commandDescriptor = CreateCommand(
            "Test",
            "",
            new ICommandParameterDescriptor[]
            {
                CreateCommandOption(typeof(string), "foo", new [] { 'f' }, "Foo option", CoconaDefaultValue.None),
                CreateCommandOption(typeof(bool), "looooooong-option", new [] { 'l' }, "Long name option", new CoconaDefaultValue(false)),
            },
            CommandFlags.Primary
        );

        var provider = new CoconaCommandMarkdownProvider(new FakeApplicationMetadataProvider() { Description = "via metadata" }, new ServiceCollection().BuildServiceProvider());
        var text = provider.CreateCommandsDocumentation(new CommandCollection(new[] { commandDescriptor }), Array.Empty<CommandDescriptor>());
        
        text.Should().Be(@"
# Description
via metadata
# Test

## Command structure
```bash
ExeName [--foo <String>] [--looooooong-option]
```
## Options
- -f, --foo <String>: Foo option (Required)
- -l, --looooooong-option: Long name option
".TrimStart());
    }

    [Fact]
    public void CommandsHelp_Single_NoParams_Rendered()
    {
        var commandDescriptor = CreateCommand(
            "Test",
            "",
            new ICommandParameterDescriptor[0],
            CommandFlags.Primary
        );

        var provider = new CoconaCommandMarkdownProvider(new FakeApplicationMetadataProvider() { Description = "via metadata" }, new ServiceCollection().BuildServiceProvider());
        var text = provider.CreateCommandsDocumentation(new CommandCollection(new[] { commandDescriptor }), Array.Empty<CommandDescriptor>());
        
        text.Should().Be(@"
# Description
via metadata
# Test

## Command structure
```bash
ExeName
```
".TrimStart());
    }

    [Fact]
    public void CommandHelp_Rendered()
    {
        var commandDescriptor = CreateCommand(
            "Test",
            "command description",
            new ICommandParameterDescriptor[]
            {
                CreateCommandOption(typeof(string), "foo", new [] { 'f' }, "Foo option", CoconaDefaultValue.None),
                CreateCommandOption(typeof(bool), "looooooong-option", new [] { 'l' }, "Long name option", new CoconaDefaultValue(false)),
            }
        );

        var provider = new CoconaCommandMarkdownProvider(new FakeApplicationMetadataProvider(), new ServiceCollection().BuildServiceProvider());
        var text = provider.CreateCommandDocumentation(commandDescriptor, Array.Empty<CommandDescriptor>());
        text.Should().Be(@"
# Test

## Description
command description
## Command structure
```bash
ExeName Test [--foo <String>] [--looooooong-option]
```
## Options
- -f, --foo <String>: Foo option (Required)
- -l, --looooooong-option: Long name option
".TrimStart());
    }

    [Fact]
    public void CommandHelp_Nested_Rendered()
    {
        var commandDescriptor = CreateCommand(
            "Test",
            "command description",
            new ICommandParameterDescriptor[]
            {
                CreateCommandOption(typeof(string), "foo", new [] { 'f' }, "Foo option", CoconaDefaultValue.None),
                CreateCommandOption(typeof(bool), "looooooong-option", new [] { 'l' }, "Long name option", new CoconaDefaultValue(false)),
            }
        );

        var subCommandStack = new[] { CreateCommand("Nested", "", Array.Empty<ICommandParameterDescriptor>()) };
        var provider = new CoconaCommandMarkdownProvider(new FakeApplicationMetadataProvider(), new ServiceCollection().BuildServiceProvider());
        var text = provider.CreateCommandDocumentation(commandDescriptor, subCommandStack);
        text.Should().Be(@"
# Test

## Description
command description
## Command structure
```bash
ExeName Nested Test [--foo <String>] [--looooooong-option]
```
## Options
- -f, --foo <String>: Foo option (Required)
- -l, --looooooong-option: Long name option
".TrimStart());
    }

    [Fact]
    public void CommandHelp_Arguments_Rendered()
    {
        var commandDescriptor = CreateCommand(
            "Test",
            "command description",
            new ICommandParameterDescriptor[]
            {
                CreateCommandOption(typeof(string), "foo", new [] { 'f' }, "Foo option", CoconaDefaultValue.None),
                CreateCommandOption(typeof(bool), "looooooong-option", new [] { 'l' }, "Long name option", new CoconaDefaultValue(false)),
                new CommandArgumentDescriptor(typeof(string[]), "src", 0, "src files", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                new CommandArgumentDescriptor(typeof(string), "dest", 0, "dest dir", CoconaDefaultValue.None, Array.Empty<Attribute>()),
            }
        );

        var provider = new CoconaCommandMarkdownProvider(new FakeApplicationMetadataProvider(), new ServiceCollection().BuildServiceProvider());
        var text = provider.CreateCommandDocumentation(commandDescriptor, Array.Empty<CommandDescriptor>());
        text.Should().Be(@"
# Test

## Description
command description
## Command structure
```bash
ExeName Test [--foo <String>] [--looooooong-option] src0 ... srcN dest
```
## Arguments
1. **src**: src files (Required)
2. **dest**: dest dir (Required)
## Options
- -f, --foo <String>: Foo option (Required)
- -l, --looooooong-option: Long name option
".TrimStart());
    }

    [Fact]
    public void CommandHelp_Arguments_Nullable_Rendered()
    {
        var commandDescriptor = CreateCommand(
            "Test",
            "command description",
            new ICommandParameterDescriptor[]
            {
                new CommandArgumentDescriptor(typeof(int), "arg0-int-not-null", 0, "Int NotNull", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                new CommandArgumentDescriptor(typeof(int?), "arg1-int-nullable", 0, "Int Nullable", new CoconaDefaultValue(null), Array.Empty<Attribute>()),
                new CommandArgumentDescriptor(typeof(string), "arg2-string-not-null", 0, "String NotNull", CoconaDefaultValue.None, Array.Empty<Attribute>()),
                new CommandArgumentDescriptor(typeof(string), "arg3-string-nullable", 0, "String Nullable", new CoconaDefaultValue(null), Array.Empty<Attribute>()),
            }
        );

        var provider = new CoconaCommandMarkdownProvider(new FakeApplicationMetadataProvider(), new ServiceCollection().BuildServiceProvider());
        var text = provider.CreateCommandDocumentation(commandDescriptor, Array.Empty<CommandDescriptor>());
        text.Should().Be(@"
# Test

## Description
command description
## Command structure
```bash
ExeName Test arg0-int-not-null arg1-int-nullable arg2-string-not-null arg3-string-nullable
```
## Arguments
1. **arg0-int-not-null**: Int NotNull (Required)
2. **arg1-int-nullable**: Int Nullable
3. **arg2-string-not-null**: String NotNull (Required)
4. **arg3-string-nullable**: String Nullable
".TrimStart());
    }

    [Fact]
    public void CreateCommandsHelp_Rendered()
    {
        var commandDescriptor = CreateCommand(
            "Test",
            "command description",
            new ICommandParameterDescriptor[]
            {
                CreateCommandOption(typeof(string), "foo", new [] { 'f' }, "Foo option", CoconaDefaultValue.None),
                CreateCommandOption(typeof(bool), "looooooong-option", new [] { 'l' }, "Long name option", new CoconaDefaultValue(false)),
            },
            CommandFlags.Primary
        );

        var provider = new CoconaCommandMarkdownProvider(new FakeApplicationMetadataProvider(), new ServiceCollection().BuildServiceProvider());
        var text = provider.CreateCommandsDocumentation(new CommandCollection(new[] { commandDescriptor }), Array.Empty<CommandDescriptor>());
        
        text.Should().Be(@"
# Test

## Description
command description
## Command structure
```bash
ExeName [--foo <String>] [--looooooong-option]
```
## Options
- -f, --foo <String>: Foo option (Required)
- -l, --looooooong-option: Long name option
".TrimStart());
    }

    [Fact]
    public void CreateCommandsHelp_Nested_Rendered()
    {
        var commandDescriptor = CreateCommand(
            "Test",
            "command description",
            new ICommandParameterDescriptor[]
            {
                CreateCommandOption(typeof(string), "foo", new [] { 'f' }, "Foo option", CoconaDefaultValue.None),
                CreateCommandOption(typeof(bool), "looooooong-option", new [] { 'l' }, "Long name option", new CoconaDefaultValue(false)),
            },
            CommandFlags.Primary
        );

        var subCommandStack = new[] {CreateCommand("Nested", "", Array.Empty<ICommandParameterDescriptor>())};
        var provider = new CoconaCommandMarkdownProvider(new FakeApplicationMetadataProvider(), new ServiceCollection().BuildServiceProvider());
        var text = provider.CreateCommandsDocumentation(new CommandCollection(new[] { commandDescriptor }), subCommandStack);
        
        text.Should().Be(@"
# Test

## Description
command description
## Command structure
```bash
ExeName Nested [--foo <String>] [--looooooong-option]
```
## Options
- -f, --foo <String>: Foo option (Required)
- -l, --looooooong-option: Long name option
".TrimStart());
    }

    [Fact]
    public void CreateCommandsHelp_Arguments_Rendered()
    {
        var commandDescriptor = CreateCommand(
            "Test",
            "command description",
            new ICommandParameterDescriptor[]
            {
                CreateCommandOption(typeof(string), "foo", new [] { 'f' }, "Foo option", CoconaDefaultValue.None),
                CreateCommandOption(typeof(bool), "looooooong-option", new [] { 'l' }, "Long name option", new CoconaDefaultValue(false)),
                new CommandArgumentDescriptor(typeof(string), "arg0", 0, "Argument description", CoconaDefaultValue.None, Array.Empty<Attribute>()),
            },
            CommandFlags.Primary
        );

        var provider = new CoconaCommandMarkdownProvider(new FakeApplicationMetadataProvider(), new ServiceCollection().BuildServiceProvider());
        var text = provider.CreateCommandsDocumentation(new CommandCollection(new[] { commandDescriptor }), Array.Empty<CommandDescriptor>());
        
        text.Should().Be(@"
# Test

## Description
command description
## Command structure
```bash
ExeName [--foo <String>] [--looooooong-option] arg0
```
## Arguments
1. **arg0**: Argument description (Required)
## Options
- -f, --foo <String>: Foo option (Required)
- -l, --looooooong-option: Long name option
".TrimStart());
    }

    [Fact]
    public void CreateCommandsHelp_Commands_Rendered()
    {
        var commandDescriptor = CreateCommand(
            "Test",
            "command description",
            new ICommandParameterDescriptor[]
            {
                CreateCommandOption(typeof(string), "foo", new [] { 'f' }, "Foo option", CoconaDefaultValue.None),
                CreateCommandOption(typeof(bool), "looooooong-option", new [] { 'l' }, "Long name option", new CoconaDefaultValue(false)),
                CreateCommandOption(typeof(int), "bar", new [] { 'b' }, "has default value", new CoconaDefaultValue(123)),
            },
            CommandFlags.Primary
        );
        var commandDescriptor2 = CreateCommand(
            "Test2",
            "command2 description",
            new ICommandParameterDescriptor[0],
            CommandFlags.None
        );

        var provider = new CoconaCommandMarkdownProvider(new FakeApplicationMetadataProvider(), new ServiceCollection().BuildServiceProvider());
        var text = provider.CreateCommandsDocumentation(new CommandCollection(new[] { commandDescriptor, commandDescriptor2 }), Array.Empty<CommandDescriptor>());
        
        text.Should().Be(@"
# Test (Default)

## Description
command description
## Command structure
```bash
ExeName [--foo <String>] [--looooooong-option] [--bar <Int32>]
```
## Options
- -f, --foo <String>: Foo option (Required)
- -l, --looooooong-option: Long name option
- -b, --bar <Int32>: has default value (Default: 123)
# Test2

## Description
command2 description
## Command structure
```bash
ExeName Test2
```
".TrimStart());
    }

    [Fact]
    public void CreateCommandsHelp_Heading2()
    {
        var commandDescriptor = CreateCommand(
            "Test",
            "command description",
            new ICommandParameterDescriptor[]
            {
                CreateCommandOption(typeof(string), "foo", new [] { 'f' }, "Foo option", CoconaDefaultValue.None),
                CreateCommandOption(typeof(bool), "looooooong-option", new [] { 'l' }, "Long name option", new CoconaDefaultValue(false)),
                CreateCommandOption(typeof(int), "bar", new [] { 'b' }, "has default value", new CoconaDefaultValue(123)),
            },
            CommandFlags.Primary
        );
        var commandDescriptor2 = CreateCommand(
            "Test2",
            "command2 description",
            new ICommandParameterDescriptor[0],
            CommandFlags.None
        );

        var provider = new CoconaCommandMarkdownProvider(new FakeApplicationMetadataProvider() { Description = "via metadata" }, new ServiceCollection().BuildServiceProvider(), CoconaCommandMarkdownProvider.HeadingLevel.Heading2);
        var text = provider.CreateCommandsDocumentation(new CommandCollection(new[] { commandDescriptor, commandDescriptor2 }), Array.Empty<CommandDescriptor>());

        text.Should().Be(@"
## Description
via metadata
## Test (Default)

### Description
command description
### Command structure
```bash
ExeName [--foo <String>] [--looooooong-option] [--bar <Int32>]
```
### Options
- -f, --foo <String>: Foo option (Required)
- -l, --looooooong-option: Long name option
- -b, --bar <Int32>: has default value (Default: 123)
## Test2

### Description
command2 description
### Command structure
```bash
ExeName Test2
```
".TrimStart());
    }

    [Fact]
    public void CreateCommandsHelp_Heading3()
    {
        var commandDescriptor = CreateCommand(
            "Test",
            "command description",
            new ICommandParameterDescriptor[]
            {
                CreateCommandOption(typeof(string), "foo", new [] { 'f' }, "Foo option", CoconaDefaultValue.None),
                CreateCommandOption(typeof(bool), "looooooong-option", new [] { 'l' }, "Long name option", new CoconaDefaultValue(false)),
                CreateCommandOption(typeof(int), "bar", new [] { 'b' }, "has default value", new CoconaDefaultValue(123)),
            },
            CommandFlags.Primary
        );
        var commandDescriptor2 = CreateCommand(
            "Test2",
            "command2 description",
            new ICommandParameterDescriptor[0],
            CommandFlags.None
        );

        var provider = new CoconaCommandMarkdownProvider(new FakeApplicationMetadataProvider() { Description = "via metadata" }, new ServiceCollection().BuildServiceProvider(), CoconaCommandMarkdownProvider.HeadingLevel.Heading3);
        var text = provider.CreateCommandsDocumentation(new CommandCollection(new[] { commandDescriptor, commandDescriptor2 }), Array.Empty<CommandDescriptor>());

        text.Should().Be(@"
### Description
via metadata
### Test (Default)

#### Description
command description
#### Command structure
```bash
ExeName [--foo <String>] [--looooooong-option] [--bar <Int32>]
```
#### Options
- -f, --foo <String>: Foo option (Required)
- -l, --looooooong-option: Long name option
- -b, --bar <Int32>: has default value (Default: 123)
### Test2

#### Description
command2 description
#### Command structure
```bash
ExeName Test2
```
".TrimStart());
    }

    [Fact]
    public void CreateCommandsHelp_Heading4()
    {
        var commandDescriptor = CreateCommand(
            "Test",
            "command description",
            new ICommandParameterDescriptor[]
            {
                CreateCommandOption(typeof(string), "foo", new [] { 'f' }, "Foo option", CoconaDefaultValue.None),
                CreateCommandOption(typeof(bool), "looooooong-option", new [] { 'l' }, "Long name option", new CoconaDefaultValue(false)),
                CreateCommandOption(typeof(int), "bar", new [] { 'b' }, "has default value", new CoconaDefaultValue(123)),
            },
            CommandFlags.Primary
        );
        var commandDescriptor2 = CreateCommand(
            "Test2",
            "command2 description",
            new ICommandParameterDescriptor[0],
            CommandFlags.None
        );

        var provider = new CoconaCommandMarkdownProvider(new FakeApplicationMetadataProvider() { Description = "via metadata" }, new ServiceCollection().BuildServiceProvider(), CoconaCommandMarkdownProvider.HeadingLevel.Heading4);
        var text = provider.CreateCommandsDocumentation(new CommandCollection(new[] { commandDescriptor, commandDescriptor2 }), Array.Empty<CommandDescriptor>());

        text.Should().Be(@"
#### Description
via metadata
#### Test (Default)

##### Description
command description
##### Command structure
```bash
ExeName [--foo <String>] [--looooooong-option] [--bar <Int32>]
```
##### Options
- -f, --foo <String>: Foo option (Required)
- -l, --looooooong-option: Long name option
- -b, --bar <Int32>: has default value (Default: 123)
#### Test2

##### Description
command2 description
##### Command structure
```bash
ExeName Test2
```
".TrimStart());
    }

    [Fact]
    public void CreateCommandsHelp_Heading5()
    {
        var commandDescriptor = CreateCommand(
            "Test",
            "command description",
            new ICommandParameterDescriptor[]
            {
                CreateCommandOption(typeof(string), "foo", new [] { 'f' }, "Foo option", CoconaDefaultValue.None),
                CreateCommandOption(typeof(bool), "looooooong-option", new [] { 'l' }, "Long name option", new CoconaDefaultValue(false)),
                CreateCommandOption(typeof(int), "bar", new [] { 'b' }, "has default value", new CoconaDefaultValue(123)),
            },
            CommandFlags.Primary
        );
        var commandDescriptor2 = CreateCommand(
            "Test2",
            "command2 description",
            new ICommandParameterDescriptor[0],
            CommandFlags.None
        );

        var provider = new CoconaCommandMarkdownProvider(new FakeApplicationMetadataProvider() { Description = "via metadata" }, new ServiceCollection().BuildServiceProvider(), CoconaCommandMarkdownProvider.HeadingLevel.Heading5);
        var text = provider.CreateCommandsDocumentation(new CommandCollection(new[] { commandDescriptor, commandDescriptor2 }), Array.Empty<CommandDescriptor>());

        text.Should().Be(@"
##### Description
via metadata
##### Test (Default)

###### Description
command description
###### Command structure
```bash
ExeName [--foo <String>] [--looooooong-option] [--bar <Int32>]
```
###### Options
- -f, --foo <String>: Foo option (Required)
- -l, --looooooong-option: Long name option
- -b, --bar <Int32>: has default value (Default: 123)
##### Test2

###### Description
command2 description
###### Command structure
```bash
ExeName Test2
```
".TrimStart());
    }

    [Fact]
    public void CreateCommandsHelp_Nested_Commands_Rendered()
    {
        var commandDescriptor = CreateCommand(
            "Test",
            "command description",
            new ICommandParameterDescriptor[]
            {
                CreateCommandOption(typeof(string), "foo", new [] { 'f' }, "Foo option", CoconaDefaultValue.None),
                CreateCommandOption(typeof(bool), "looooooong-option", new [] { 'l' }, "Long name option", new CoconaDefaultValue(false)),
                CreateCommandOption(typeof(int), "bar", new [] { 'b' }, "has default value", new CoconaDefaultValue(123)),
            },
            CommandFlags.Primary
        );
        var commandDescriptor2 = CreateCommand(
            "Test2",
            "command2 description",
            new ICommandParameterDescriptor[0],
            CommandFlags.None
        );

        var subCommandStack = new[] { CreateCommand("Nested", "", Array.Empty<ICommandParameterDescriptor>()) };
        var provider = new CoconaCommandMarkdownProvider(new FakeApplicationMetadataProvider(), new ServiceCollection().BuildServiceProvider());
        var text = provider.CreateCommandsDocumentation(new CommandCollection(new[] { commandDescriptor, commandDescriptor2 }), subCommandStack);
        
        text.Should().Be(@"
# Test (Default)

## Description
command description
## Command structure
```bash
ExeName Nested [--foo <String>] [--looooooong-option] [--bar <Int32>]
```
## Options
- -f, --foo <String>: Foo option (Required)
- -l, --looooooong-option: Long name option
- -b, --bar <Int32>: has default value (Default: 123)
# Test2

## Description
command2 description
## Command structure
```bash
ExeName Nested Test2
```
".TrimStart());
    }

    [Fact]
    public void CreateCommandsHelp_Commands_NoOptionInPrimary_Rendered()
    {
        var commandDescriptor = CreateCommand(
            "Test",
            "command description",
            new ICommandParameterDescriptor[0],
            CommandFlags.Primary
        );
        var commandDescriptor2 = CreateCommand(
            "Test2",
            "command2 description",
            new ICommandParameterDescriptor[0],
            CommandFlags.None
        );

        var provider = new CoconaCommandMarkdownProvider(new FakeApplicationMetadataProvider(), new ServiceCollection().BuildServiceProvider());
        var text = provider.CreateCommandsDocumentation(new CommandCollection(new[] { commandDescriptor, commandDescriptor2 }), Array.Empty<CommandDescriptor>());
        
        text.Should().Be(@"# Test (Default)

## Description
command description
## Command structure
```bash
ExeName
```
# Test2

## Description
command2 description
## Command structure
```bash
ExeName Test2
```
".TrimStart());
    }

    [Fact]
    public void CreateCommandsHelp_Commands_Hidden_Rendered()
    {
        var commandDescriptor = CreateCommand(
            "Test",
            "command description",
            new ICommandParameterDescriptor[0],
            CommandFlags.Hidden
        );
        var commandDescriptor2 = CreateCommand(
            "Test2",
            "command2 description",
            new ICommandParameterDescriptor[0],
            CommandFlags.None
        );

        var provider = new CoconaCommandMarkdownProvider(new FakeApplicationMetadataProvider(), new ServiceCollection().BuildServiceProvider());
        var text = provider.CreateCommandsDocumentation(new CommandCollection(new[] { commandDescriptor, commandDescriptor2 }), Array.Empty<CommandDescriptor>());
        
        text.Should().Be(@"
# Test

## Description
command description
## Command structure
```bash
ExeName Test
```
# Test2

## Description
command2 description
## Command structure
```bash
ExeName Test2
```
".TrimStart());
    }

    [Fact]
    public void CommandHelp_Options_Enum_Rendered()
    {
        var commandDescriptor = CreateCommand(
            "Test",
            "command description",
            new ICommandParameterDescriptor[]
            {
                CreateCommandOption(typeof(CommandHelpEnumValue), "enum", new [] { 'e' }, "Enum option", CoconaDefaultValue.None),
            }
        );

        var provider = new CoconaCommandMarkdownProvider(new FakeApplicationMetadataProvider(), new ServiceCollection().BuildServiceProvider());
        var text = provider.CreateCommandDocumentation(commandDescriptor, Array.Empty<CommandDescriptor>());
        text.Should().Be(@"
# Test

## Description
command description
## Command structure
```bash
ExeName Test [--enum <CommandHelpEnumValue>]
```
## Options
- -e, --enum <CommandHelpEnumValue>: Enum option (Required) (Allowed values: Alice, Karen, Other)
".TrimStart());
    }

    [Fact]
    public void CommandHelp_Options_Boolean_DefaultFalse_Rendered()
    {
        var commandDescriptor = CreateCommand(
            "Test",
            "command description",
            new ICommandParameterDescriptor[]
            {
                CreateCommandOption(typeof(bool), "flag", new [] { 'f' }, "Boolean option", new CoconaDefaultValue(false)),
            }
        );

        var provider = new CoconaCommandMarkdownProvider(new FakeApplicationMetadataProvider(), new ServiceCollection().BuildServiceProvider());
        var text = provider.CreateCommandDocumentation(commandDescriptor, Array.Empty<CommandDescriptor>());
        text.Should().Be(@"
# Test

## Description
command description
## Command structure
```bash
ExeName Test [--flag]
```
## Options
- -f, --flag: Boolean option
".TrimStart());
    }

    [Fact]
    public void CommandHelp_Options_Boolean_DefaultTrue_Rendered()
    {
        var commandDescriptor = CreateCommand(
            "Test",
            "command description",
            new ICommandParameterDescriptor[]
            {
                CreateCommandOption(typeof(bool), "flag", new [] { 'f' }, "Boolean option", new CoconaDefaultValue(true)),
            }
        );

        var provider = new CoconaCommandMarkdownProvider(new FakeApplicationMetadataProvider(), new ServiceCollection().BuildServiceProvider());
        var text = provider.CreateCommandDocumentation(commandDescriptor, Array.Empty<CommandDescriptor>());
        text.Should().Be(@"
# Test

## Description
command description
## Command structure
```bash
ExeName Test [--flag=<true|false>]
```
## Options
- -f, --flag=<true|false>: Boolean option (Default: True)
".TrimStart());
    }

    [Fact]
    public void CommandHelp_Options_NullableBoolean_DefaultFalse_Rendered()
    {
        var commandDescriptor = CreateCommand(
            "Test",
            "command description",
            new ICommandParameterDescriptor[]
            {
                CreateCommandOption(typeof(bool?), "flag", new [] { 'f' }, "Boolean option", new CoconaDefaultValue(null)),
            }
        );

        var provider = new CoconaCommandMarkdownProvider(new FakeApplicationMetadataProvider(), new ServiceCollection().BuildServiceProvider());
        var text = provider.CreateCommandDocumentation(commandDescriptor, Array.Empty<CommandDescriptor>());
        text.Should().Be(@"
# Test

## Description
command description
## Command structure
```bash
ExeName Test [--flag]
```
## Options
- -f, --flag: Boolean option
".TrimStart());
    }

    [Fact]
    public void CommandHelp_Options_Nullable()
    {
        var commandDescriptor = CreateCommand(
            "Test",
            "command description",
            new ICommandParameterDescriptor[]
            {
                CreateCommandOption(typeof(string), "nrt", new [] { 'f' }, "Nullable Reference Type", new CoconaDefaultValue(null)),
                CreateCommandOption(typeof(bool?), "looooooong-option", new [] { 'l' }, "Long name option", new CoconaDefaultValue(null)),
                CreateCommandOption(typeof(int?), "nullable-int", new [] { 'x' }, "Nullable Int", new CoconaDefaultValue(null)),
            },
            CommandFlags.Primary
        );

        var provider = new CoconaCommandMarkdownProvider(new FakeApplicationMetadataProvider(), new ServiceCollection().BuildServiceProvider());
        var text = provider.CreateCommandsDocumentation(new CommandCollection(new[] { commandDescriptor }), Array.Empty<CommandDescriptor>());
        
        text.Should().Be(@"
# Test

## Description
command description
## Command structure
```bash
ExeName [--nrt <String>] [--looooooong-option] [--nullable-int <Int32>]
```
## Options
- -f, --nrt <String>: Nullable Reference Type
- -l, --looooooong-option: Long name option
- -x, --nullable-int <Int32>: Nullable Int
".TrimStart());
    }

    [Fact]
    public void CommandHelp_Options_Hidden_Rendered()
    {
        var commandDescriptor = CreateCommand(
            "Test",
            "command description",
            new ICommandParameterDescriptor[]
            {
                CreateCommandOption(typeof(bool), "flag", new [] { 'f' }, "Boolean option", new CoconaDefaultValue(true), CommandOptionFlags.Hidden),
            }
        );

        var provider = new CoconaCommandMarkdownProvider(new FakeApplicationMetadataProvider(), new ServiceCollection().BuildServiceProvider());
        var text = provider.CreateCommandDocumentation(commandDescriptor, Array.Empty<CommandDescriptor>());
        text.Should().Be(@"
# Test

## Description
command description
## Command structure
```bash
ExeName Test
```
".TrimStart());
    }

    [Fact]
    public void CommandHelp_Options_Array_Rendered()
    {
        var commandDescriptor = CreateCommand(
            "Test",
            "command description",
            new ICommandParameterDescriptor[]
            {
                CreateCommandOption(typeof(int[]), "option0", new [] { 'o' }, "Int option values", CoconaDefaultValue.None),
            }
        );

        var provider = new CoconaCommandMarkdownProvider(new FakeApplicationMetadataProvider(), new ServiceCollection().BuildServiceProvider());
        var text = provider.CreateCommandDocumentation(commandDescriptor, Array.Empty<CommandDescriptor>());
        text.Should().Be(@"
# Test

## Description
command description
## Command structure
```bash
ExeName Test [--option0 <Int32>...]
```
## Options
- -o, --option0 <Int32>...: Int option values (Required)
".TrimStart());
    }

    [Fact]
    public void CommandHelp_Options_Generics_Rendered()
    {
        var commandDescriptor = CreateCommand(
            "Test",
            "command description",
            new ICommandParameterDescriptor[]
            {
                CreateCommandOption(typeof(List<int>), "option0", new [] { 'o' }, "Int option values", CoconaDefaultValue.None),
            }
        );

        var provider = new CoconaCommandMarkdownProvider(new FakeApplicationMetadataProvider(), new ServiceCollection().BuildServiceProvider());
        var text = provider.CreateCommandDocumentation(commandDescriptor, Array.Empty<CommandDescriptor>());
        text.Should().Be(@"
# Test

## Description
command description
## Command structure
```bash
ExeName Test [--option0 <Int32>...]
```
## Options
- -o, --option0 <Int32>...: Int option values (Required)
".TrimStart());
    }

    public enum CommandHelpEnumValue
    {
        Alice, Karen, Other
    }
}
