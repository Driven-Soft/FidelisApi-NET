using Fidelis.Application.DTOs;
using Fidelis.Application.Interfaces.Repositories;
using Fidelis.Application.Services.Implementations;
using Fidelis.Domain.Entities;
using Moq;
using Xunit;

namespace Fidelis.UnitTests.Services;

public class VeterinarioServiceTests
{
    private readonly Mock<IVeterinarioRepository> _repositoryMock = new();
    private readonly VeterinarioService _sut;

    public VeterinarioServiceTests()
    {
        _sut = new VeterinarioService(_repositoryMock.Object);
    }

    [Fact]
    public void Create_SenhaCurta_LancaArgumentExceptionENaoPersiste()
    {
        var request = new VeterinarioRequest
        {
            Crmv = "SP-1",
            Nome = "Dra. Ana",
            Email = "ana@vet.com",
            Senha = "123",
            Especialidade = "Clínico Geral",
            ClinicaId = 1
        };

        Assert.Throws<ArgumentException>(() => _sut.Create(request));
        _repositoryMock.Verify(r => r.Add(It.IsAny<Veterinario>()), Times.Never);
    }

    [Fact]
    public void Patch_ApenasEspecialidade_MantemDemaisCampos()
    {
        var veterinario = new Veterinario("SP-1", "Dra. Ana", "ana@vet.com", "123456", "Clínico Geral", 1);
        _repositoryMock.Setup(r => r.GetById(1)).Returns(veterinario);

        var result = _sut.Patch(1, new VeterinarioPatchRequest { Especialidade = "Cardiologia" });

        Assert.Equal("Cardiologia", result!.Especialidade);
        Assert.Equal("SP-1", result.Crmv);
        _repositoryMock.Verify(r => r.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Update_VeterinarioInexistente_RetornaNull()
    {
        _repositoryMock.Setup(r => r.GetById(It.IsAny<int>())).Returns((Veterinario?)null);

        var result = _sut.Update(5, new VeterinarioRequest());

        Assert.Null(result);
    }
}
