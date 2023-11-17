using AspNetCore8MinimalApis.Models;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Routing;
using System.Text.RegularExpressions;

namespace AspNetCore8MinimalApis.Validators;

public class IFormFileValidator : AbstractValidator<IFormFile>
{
    public IFormFileValidator()
    {

        RuleFor(x => x).Must((file, context) =>
        {
            return file.ContentType == "text/csv";
        }).WithMessage("The Content-Type is not valid");

        RuleFor(x => x).Must((file, context) =>
        {
            return file.FileName.EndsWith(".csv");
        }).WithMessage("The file extension is not valid");

        RuleFor(x => x.FileName).Matches("^[A-Za-z0-9_\\-.]*$").WithMessage("The file name is not valid");

        RuleFor(x => x).Must((file, context) =>
        {
            var exeSignatures = new List<string> { "4D-5A", "5A 4D" }; // string representation of hexadecimal signature of an execute file
            BinaryReader binary = new BinaryReader(file.OpenReadStream());
            byte[] bytes = binary.ReadBytes(2);
            string fileSequenceHex = BitConverter.ToString(bytes);
            //string fileSequenceHex = fileHex.Substring(0, 4);

            foreach (var exeSignature in exeSignatures)
            {
                if (exeSignature == fileSequenceHex)
                    return false;
            }
            return true;
        }).WithName("FileContent")
          .WithMessage("The file content is not valid");
    }
}

public class CountryFileUploadValidator : AbstractValidator<CountryFileUpload>
{
    public CountryFileUploadValidator()
    {

        RuleFor(x => x.File).Must((file, context) =>
        {
            return file.File.ContentType == "text/csv";
        }).WithMessage("ContentType is not valid");

        RuleFor(x => x.File).Must((file, context) =>
        {
            return file.File.FileName.EndsWith(".csv");
        }).WithMessage("The file extension is not valid");

        RuleFor(x => x.File.FileName).Matches("^[A-Za-z_-][A-Za-z0-9_-]*$").WithMessage("The file name is not valid");

        RuleFor(x => x.File).Must((file, context) =>
        {
            var exeSignatures = new List<string> { "4D-5A", "5A 4D" }; // string representation of hexadecimal signature of an execute file
            BinaryReader binary = new BinaryReader(file.File.OpenReadStream());
            byte[] bytes = binary.ReadBytes(2);
            string fileSequenceHex = BitConverter.ToString(bytes);

            foreach (var exeSignature in exeSignatures)
                if (exeSignature.Equals(fileSequenceHex, StringComparison.OrdinalIgnoreCase))
                    return false;
            return true;
        }).WithName("FileContent")
          .WithMessage("The file content is not valid");

        RuleFor(x => x.AuthorName).NotEmpty().WithMessage("{PropertyName} is required")
                            .Custom((authorName, context) =>
                            {
                                Regex rg = new Regex("<.*?>"); // try to match HTML tags
                                if (rg.Matches(authorName).Count > 0)
                                {
                                    // Raises an error
                                    context.AddFailure(new ValidationFailure("AuthorName", "The AuthorName parameter has invalid content"));
                                }
                            });

        RuleFor(x => x.Description).NotEmpty().WithMessage("{PropertyName} is required")
                    .Custom((name, context) =>
                    {
                        Regex rg = new Regex("<.*?>"); // try to match HTML tags
                        if (rg.Matches(name).Count > 0)
                        {
                            // Raises an error
                            context.AddFailure(new ValidationFailure("Name", "The AuthorName parameter has invalid content"));
                        }
                    });
    }
}