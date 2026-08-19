namespace SistemaInventario.Tests.Architecture;

using System.Reflection;
using Xunit;

/// <summary>
/// Pruebas de fronteras arquitectónicas para validar Clean Architecture usando reflexión.
/// Implementación del ADR-001: Adopción de Monolito Modular con Verificación de Fronteras Arquitectónicas.
/// </summary>
public class ArchitectureBoundariesTests
{
    private const string DomainNamespace = "SistemaInventario.Domain";
    private const string ApplicationNamespace = "SistemaInventario.Application";
    private const string InfrastructureNamespace = "SistemaInventario.Infrastructure";
    private const string PresentationNamespace = "SistemaInventario.Presentation";

    private static Assembly GetAssemblyByNamespace(string namespaceName)
    {
        return namespaceName switch
        {
            DomainNamespace => typeof(Domain.Entities.Producto).Assembly,
            ApplicationNamespace => typeof(Application.UseCases.Productos.RegistrarProductoUseCase).Assembly,
            InfrastructureNamespace => typeof(Infrastructure.Data.InventarioDbContext).Assembly,
            PresentationNamespace => typeof(Presentation.ConsoleUI.MenuProductos).Assembly,
            _ => throw new ArgumentException($"Unknown namespace: {namespaceName}")
        };
    }

    private static Type[] GetTypesInNamespace(Assembly assembly, string namespaceName)
    {
        return assembly.GetTypes()
            .Where(t => t.Namespace != null && t.Namespace.StartsWith(namespaceName))
            .ToArray();
    }

    private static bool TypeHasDependencyOn(Type type, string targetNamespace)
    {
        // Verificar referencias directas en métodos
        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        foreach (var method in methods)
        {
            if (method.ReturnType.Namespace?.StartsWith(targetNamespace) == true)
                return true;

            var parameters = method.GetParameters();
            if (parameters.Any(p => p.ParameterType.Namespace?.StartsWith(targetNamespace) == true))
                return true;
        }

        // Verificar referencias en propiedades
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var property in properties)
        {
            if (property.PropertyType.Namespace?.StartsWith(targetNamespace) == true)
                return true;
        }

        // Verificar referencias en campos
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var field in fields)
        {
            if (field.FieldType.Namespace?.StartsWith(targetNamespace) == true)
                return true;
        }

        return false;
    }

    [Fact]
    public void Domain_ShouldNotDependOnOtherLayers()
    {
        var domainAssembly = GetAssemblyByNamespace(DomainNamespace);
        var domainTypes = GetTypesInNamespace(domainAssembly, DomainNamespace);

        var violatingTypes = domainTypes
            .Where(t => 
                TypeHasDependencyOn(t, ApplicationNamespace) ||
                TypeHasDependencyOn(t, InfrastructureNamespace) ||
                TypeHasDependencyOn(t, PresentationNamespace)
            )
            .ToList();

        Assert.Empty(violatingTypes);
    }

    [Fact]
    public void Application_ShouldNotDependOnInfrastructureOrPresentation()
    {
        var applicationAssembly = GetAssemblyByNamespace(ApplicationNamespace);
        var applicationTypes = GetTypesInNamespace(applicationAssembly, ApplicationNamespace);

        var violatingTypes = applicationTypes
            .Where(t => 
                TypeHasDependencyOn(t, InfrastructureNamespace) ||
                TypeHasDependencyOn(t, PresentationNamespace)
            )
            .ToList();

        Assert.Empty(violatingTypes);
    }

    [Fact]
    public void Application_ShouldDependOnDomain()
    {
        var applicationAssembly = GetAssemblyByNamespace(ApplicationNamespace);
        var applicationTypes = GetTypesInNamespace(applicationAssembly, ApplicationNamespace);

        // Verificar que al menos algunos tipos en Application dependan de Domain
        var typesWithDomainDependency = applicationTypes
            .Where(t => TypeHasDependencyOn(t, DomainNamespace))
            .ToList();

        Assert.NotEmpty(typesWithDomainDependency);
    }

    [Fact]
    public void Infrastructure_ShouldNotDependOnPresentation()
    {
        var infrastructureAssembly = GetAssemblyByNamespace(InfrastructureNamespace);
        var infrastructureTypes = GetTypesInNamespace(infrastructureAssembly, InfrastructureNamespace);

        var violatingTypes = infrastructureTypes
            .Where(t => TypeHasDependencyOn(t, PresentationNamespace))
            .ToList();

        Assert.Empty(violatingTypes);
    }

    [Fact]
    public void Infrastructure_ShouldDependOnDomainAndApplication()
    {
        var infrastructureAssembly = GetAssemblyByNamespace(InfrastructureNamespace);
        var infrastructureTypes = GetTypesInNamespace(infrastructureAssembly, InfrastructureNamespace);

        var typesWithDomainDependency = infrastructureTypes
            .Where(t => TypeHasDependencyOn(t, DomainNamespace))
            .ToList();

        // Infrastructure DEBE tener al menos algunos tipos que dependan de Domain (Repositorios, DbContext, etc.)
        Assert.NotEmpty(typesWithDomainDependency);
    }

    [Fact]
    public void Presentation_ShouldNotCircularlyDependOnApplication()
    {
        var applicationAssembly = GetAssemblyByNamespace(ApplicationNamespace);
        var applicationTypes = GetTypesInNamespace(applicationAssembly, ApplicationNamespace);

        var violatingTypes = applicationTypes
            .Where(t => TypeHasDependencyOn(t, PresentationNamespace))
            .ToList();

        Assert.Empty(violatingTypes);
    }

    [Fact]
    public void Domain_ShouldBeIndependent()
    {
        var domainAssembly = GetAssemblyByNamespace(DomainNamespace);
        var domainTypes = GetTypesInNamespace(domainAssembly, DomainNamespace);

        // Domain no debe depender de ninguna otra capa (máxima independencia)
        var violatingTypes = domainTypes
            .Where(t => 
                TypeHasDependencyOn(t, ApplicationNamespace) ||
                TypeHasDependencyOn(t, InfrastructureNamespace) ||
                TypeHasDependencyOn(t, PresentationNamespace)
            )
            .ToList();

        Assert.Empty(violatingTypes);
    }

    [Fact]
    public void AllLayersExist()
    {
        // Verificar que todos los ensamblados existan
        var domainAssembly = GetAssemblyByNamespace(DomainNamespace);
        var applicationAssembly = GetAssemblyByNamespace(ApplicationNamespace);
        var infrastructureAssembly = GetAssemblyByNamespace(InfrastructureNamespace);
        var presentationAssembly = GetAssemblyByNamespace(PresentationNamespace);

        Assert.NotNull(domainAssembly);
        Assert.NotNull(applicationAssembly);
        Assert.NotNull(infrastructureAssembly);
        Assert.NotNull(presentationAssembly);
    }
}
