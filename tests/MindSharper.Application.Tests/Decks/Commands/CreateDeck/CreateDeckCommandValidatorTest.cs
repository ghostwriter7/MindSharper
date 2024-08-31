using System.Linq;
using FluentAssertions;
using FluentValidation.TestHelper;
using JetBrains.Annotations;
using MindSharper.Application.Decks.Commands.CreateDeck;
using Xunit;

namespace MindSharper.Application.Tests.Decks.Commands.CreateDeck;

[TestSubject(typeof(CreateDeckCommandValidator))]
public class CreateDeckCommandValidatorTest
{

    [Fact]
    public void Validator_ForValidCommand_ShouldNotHaveAnyValidatorErrors()
    {
        var command = new CreateDeckCommand("New name");
        var validator = new CreateDeckCommandValidator();

        var validationResults = validator.TestValidate(command);
        
        validationResults.ShouldNotHaveAnyValidationErrors();
    }
    
    
    [Fact]
    public void Validator_ForNameShorterThan2Chars_ShouldHaveValidationError()
    {
        var command = new CreateDeckCommand("A");
        var validator = new CreateDeckCommandValidator();

        var validationResults = validator.TestValidate(command);
        
        validationResults.ShouldHaveValidationErrorFor(command => command.Name);
    }
    
    
    [Fact]
    public void Validator_ForNameLongerThan20Chars_ShouldHaveValidationError()
    {
        var command = new CreateDeckCommand(new string(Enumerable.Repeat('a', 21).ToArray()));
        var validator = new CreateDeckCommandValidator();

        var validationResults = validator.TestValidate(command);
        
        validationResults.ShouldHaveValidationErrorFor(command => command.Name);
    }
}