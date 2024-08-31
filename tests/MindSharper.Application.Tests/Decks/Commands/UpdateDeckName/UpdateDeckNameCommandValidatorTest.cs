using System.Linq;
using FluentValidation.TestHelper;
using JetBrains.Annotations;
using MindSharper.Application.Decks.Commands.UpdateDeckName;
using Xunit;

namespace MindSharper.Application.Tests.Decks.Commands.UpdateDeckName;

[TestSubject(typeof(UpdateDeckNameCommandValidator))]
public class UpdateDeckNameCommandValidatorTest
{

    [Fact]
    public void Validator_ForValidCommand_ShouldNotHaveAnyValidatorErrors()
    {
        var command = new UpdateDeckNameCommand(100, "New name");
        var validator = new UpdateDeckNameCommandValidator();

        var validationResults = validator.TestValidate(command);
        
        validationResults.ShouldNotHaveAnyValidationErrors();
    }
    
    
    [Fact]
    public void Validator_ForNameShorterThan2Chars_ShouldHaveValidationError()
    {
        var command = new UpdateDeckNameCommand(100, "A");
        var validator = new UpdateDeckNameCommandValidator();

        var validationResults = validator.TestValidate(command);
        
        validationResults.ShouldHaveValidationErrorFor(command => command.Name);
    }
    
    
    [Fact]
    public void Validator_ForNameLongerThan20Chars_ShouldHaveValidationError()
    {
        var command = new UpdateDeckNameCommand(100, new string(Enumerable.Repeat('a', 21).ToArray()));
        var validator = new UpdateDeckNameCommandValidator();

        var validationResults = validator.TestValidate(command);
        
        validationResults.ShouldHaveValidationErrorFor(command => command.Name);
    }
}