using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.ParcoursExceptions;
using UniversiteDomain.Util;

namespace UniversiteDomain.UseCases.ParcoursUseCases.Create;

public class CreateParcoursUseCase(IRepositoryFactory parcoursRepository)
{
    public async Task<Parcours> ExecuteAsync(string nomParcours, int anneeFormation)
    {
        var parcours = new Parcours{NomParcours = nomParcours, AnneeFormation = anneeFormation};
        return await ExecuteAsync(parcours);
    }
    public async Task<Parcours> ExecuteAsync(Parcours parcours)
    {
        await CheckBusinessRules(parcours);
        Parcours et = await parcoursRepository.ParcoursRepository().CreateAsync(parcours);
        parcoursRepository.SaveChangesAsync().Wait();
        return et;
    }
    
    private async Task CheckBusinessRules(Parcours parcours)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentNullException.ThrowIfNull(parcours.NomParcours);
        ArgumentNullException.ThrowIfNull(parcoursRepository);
        
        // On recherche un étudiant avec le même numéro étudiant
        // On recherche un parcour avec le même nom et la; même année de formation
        List<Parcours> existe = await parcoursRepository.ParcoursRepository().FindByConditionAsync(e=>e.NomParcours.Equals(parcours.NomParcours) && e.AnneeFormation.Equals(parcours.AnneeFormation));

        // Si un parcours avec le même nom et la même année de formation existe déjà, on lève une exception personnalisée
        if (existe is {Count:>0}) throw new DuplicateParcoursException(parcours.ToString()+ " - ce parcours existe déjà");

        // Le métier définit que le nom doit contenir plus de 1 lettre
        if (parcours.NomParcours.Length < 1) throw new InvalidNomParcoursException(parcours.NomParcours +" incorrect - Le nom du parcours doit contenir au moins 1 caractère");
        
        // Le métier définit que l'année de formation doit ếtre égale à 1 ou 2 
        if (!parcours.AnneeFormation.Equals(1) && !parcours.AnneeFormation.Equals(2)) throw new InvalidAnneeFormationException(parcours.AnneeFormation +" incorrect - L'année de formation doit être 1 ou 2");
    }
}