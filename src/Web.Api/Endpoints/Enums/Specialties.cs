using SharedKernel;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Enums;

internal sealed class Specialties : IEndpoint
{
    public static readonly Dictionary<string, string[]> SpecialtyData = new()
    {
        { "Anaesthesiology", new[] { "Cardiothoracic Anaesthesia", "Critical Care Medicine", "Neuro-Anaesthesia", "Obstetric Anaesthesia", "Pain Medicine", "Paediatric Anaesthesia" } },
        { "Biomedical Informatics", Array.Empty<string>() },
        { "Chemical Pathology", Array.Empty<string>() },
        { "Clinical Genetics", Array.Empty<string>() },
        { "Clinical Nutrition", Array.Empty<string>() },
        { "Clinical Oncology", Array.Empty<string>() },
        { "Clinical Pharmacology & Therapeutics", Array.Empty<string>() },
        { "Clinical Radiology", new[] { "Cardiothoracic Imaging", "Interventional Radiology", "Neuroradiology", "Paediatric Radiology", "Women's Imaging (Breast & Obstetric Imaging)" } },
        { "Community Medicine", Array.Empty<string>() },
        { "Critical Care Medicine", Array.Empty<string>() },
        { "Dermatology", Array.Empty<string>() },
        { "Developmental Paediatrics", Array.Empty<string>() },
        { "Family Medicine", Array.Empty<string>() },
        { "Forensic Medicine", Array.Empty<string>() },
        { "General Medicine (Internal Medicine)", new[] { "Adult Cardiology", "Cardiac Electrophysiology", "Endocrinology & Metabolic Medicine", "Gastroenterology", "Geriatric Medicine", "Nephrology", "Neurology", "Clinical Neurophysiology", "Respiratory Medicine (Pulmonology)", "Rheumatology & Rehabilitation Medicine" } },
        { "General Surgery", new[] { "Cardiothoracic Surgery", "Gastrointestinal Surgery", "Neuro-surgery", "Paediatric Surgery", "Plastic & Reconstructive Surgery", "Thoracic Surgery", "Urological Surgery", "Vascular Surgery" } },
        { "Haematology", Array.Empty<string>() },
        { "Histopathology", Array.Empty<string>() },
        { "Immunology", Array.Empty<string>() },
        { "Medical Administration", Array.Empty<string>() },
        { "Medical Education", Array.Empty<string>() },
        { "Medical Informatics", Array.Empty<string>() },
        { "Medical Microbiology", Array.Empty<string>() },
        { "Medical Virology", Array.Empty<string>() },
        { "Obstetrics & Gynaecology", new[] { "Gynaecological Oncology", "Maternal Fetal Medicine (Perinatology)", "Reproductive Medicine", "Urogynaecology" } },
        { "Ophthalmology", Array.Empty<string>() },
        { "Orthopaedic Surgery", Array.Empty<string>() },
        { "Otorhinolaryngology (ENT Surgery)", Array.Empty<string>() },
        { "Paediatrics", new[] { "Developmental Paediatrics", "Paediatric Cardiology", "Paediatric Endocrinology", "Paediatric Gastroenterology", "Paediatric Intensive Care", "Paediatric Neonatology", "Paediatric Nephrology", "Paediatric Neurology", "Paediatric Respiratory Medicine" } },
        { "Palliative Medicine", Array.Empty<string>() },
        { "Psychiatry", new[] { "Addiction Psychiatry", "Child and Adolescent Psychiatry", "Forensic Psychiatry", "Old Age Psychiatry" } },
        { "Sports Medicine", Array.Empty<string>() },
        { "Transfusion Medicine", Array.Empty<string>() }
    };

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/enums/specialties", () =>
        {
            return Result.Success(SpecialtyData).ToApiResponse();
        })
        .RequireAuthorization()
        .WithTags("Enums");
    }
}
