using Elsa.Agents;

namespace Elsa.Server.Web.Skills;

public class ErpSkillsProvider : ISkillsProvider
{
    public IEnumerable<SkillDescriptor> GetSkills()
    {
        yield return SkillDescriptor.From<EstoqueSkill>("ConsultarEstoque");
        yield return SkillDescriptor.From<PedidosSkill>("ConsultarPedidos");
        yield return SkillDescriptor.From<FornecedoresSkill>("ConsultarFornecedores");
    }
}
