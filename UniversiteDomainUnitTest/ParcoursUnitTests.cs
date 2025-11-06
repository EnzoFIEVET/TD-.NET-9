using System.Linq.Expressions;
using Moq;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.ParcoursUseCases;
using UniversiteDomain.UseCases.ParcoursUseCases.Create;

namespace UniversiteDomainUnitTest;

public class ParcoursUnitTest
{
    [SetUp]
    public void Setup()
    {
    }
    [Test]
    public async Task CreateParcoursUseCase()
    {
        long id = 1;
        string nom = "Bases de Données Avancées";
        int annee = 1;
        
        // On crée le parcours qui doit être ajouté en base
        Parcours parcoursSansId = new Parcours{NomParcours= nom, AnneeFormation = annee};
        //  Créons le mock du repository
        // On initialise une fausse datasource qui va simuler un ParcoursRepository
        var mock = new Mock<IParcoursRepository>();
        // Il faut ensuite aller dans le use case pour voir quelles fonctions simuler
        // Nous devons simuler FindByCondition et Create
        
        // Simulation de la fonction FindByCondition
        // On dit à ce mock que le parcours n'existe pas déjà
        // La réponse à l'appel FindByCondition est donc une liste vide
        var reponseFindByCondition = new List<Parcours>();
        // On crée un bouchon dans le mock pour la fonction FindByCondition
        // Quelque soit le paramètre de la fonction FindByCondition, on renvoie la liste vide
        mock.Setup(repo=>repo.FindByConditionAsync(It.IsAny<Expression<Func<Parcours, bool>>>())).ReturnsAsync(reponseFindByCondition);
        
        // Simulation de la fonction Create
        // On lui dit que l'ajout du parcours renvoie un parcours avec l'Id 1
        Parcours parcoursCree =new Parcours{Id=id, NomParcours = nom, AnneeFormation = annee};
        mock.Setup(repoParcours=>repoParcours.CreateAsync(parcoursSansId)).ReturnsAsync(parcoursCree);
        
        // On crée le bouchon (un faux parcoursRepository). Il est prêt à être utilisé
        var fauxParcoursRepository = mock.Object;
        
        // Création du use case en injectant notre faux repository
        CreateParcoursUseCase useCase=new CreateParcoursUseCase(fauxParcoursRepository);
        // Appel du use case
        var parcoursTeste=await useCase.ExecuteAsync(parcoursSansId);
        
        // Vérification du résultat
        Assert.That(parcoursTeste.Id, Is.EqualTo(parcoursCree.Id));
        Assert.That(parcoursTeste.NomParcours, Is.EqualTo(parcoursCree.NomParcours));
        Assert.That(parcoursTeste.AnneeFormation, Is.EqualTo(parcoursCree.AnneeFormation));
    }

}