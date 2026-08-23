using RutaCero.Application.Common;
using RutaCero.Domain.Transactions;

namespace RutaCero.Application.Transactions;

public sealed record TransactionTypeSettingDto(string Code,string Label,string Effect);

public interface ITransactionTypeSettingRepository
{
    Task<IReadOnlyList<TransactionTypeSetting>> ListAsync(Guid userId,CancellationToken token);
    Task<TransactionTypeSetting?> FindAsync(Guid userId,TransactionType code,CancellationToken token);
    Task AddAsync(TransactionTypeSetting setting,CancellationToken token);
}

public sealed class TransactionTypeSettingService(ITransactionTypeSettingRepository settings,IUnitOfWork unitOfWork)
{
    private static readonly IReadOnlyDictionary<TransactionType,(string Label,TransactionEffect Effect)> Defaults=
        new Dictionary<TransactionType,(string,TransactionEffect)>{
            [TransactionType.Income]=("Ingreso",TransactionEffect.Positive),[TransactionType.Expense]=("Gasto",TransactionEffect.Negative),
            [TransactionType.Transfer]=("Transferencia",TransactionEffect.Neutral),[TransactionType.DebtPayment]=("Pago de deuda",TransactionEffect.Negative),
            [TransactionType.Interest]=("Interés",TransactionEffect.Negative),[TransactionType.Fee]=("Comisión",TransactionEffect.Negative),
            [TransactionType.Refund]=("Reembolso",TransactionEffect.Positive),[TransactionType.Adjustment]=("Ajuste",TransactionEffect.Neutral)};

    public async Task<IReadOnlyList<TransactionTypeSettingDto>> ListAsync(Guid userId,CancellationToken token)
    {
        var saved=(await settings.ListAsync(userId,token)).ToDictionary(x=>x.Code);
        foreach(var item in Defaults.Where(x=>!saved.ContainsKey(x.Key)))
        {
            var setting=new TransactionTypeSetting(userId,item.Key,item.Value.Label,item.Value.Effect);
            await settings.AddAsync(setting,token);saved[item.Key]=setting;
        }
        await unitOfWork.SaveChangesAsync(token);
        return Defaults.Select(x=>Map(saved[x.Key])).ToList();
    }

    public async Task<TransactionTypeSettingDto?> UpdateAsync(Guid userId,string code,string label,string effect,CancellationToken token)
    {
        if(!Enum.TryParse<TransactionType>(code,true,out var parsed)||!Defaults.ContainsKey(parsed)
            ||!Enum.TryParse<TransactionEffect>(effect,true,out var parsedEffect)||string.IsNullOrWhiteSpace(label))return null;
        var item=await settings.FindAsync(userId,parsed,token);
        if(item is null){item=new TransactionTypeSetting(userId,parsed,label,parsedEffect);await settings.AddAsync(item,token);}
        else item.Update(label,parsedEffect);
        await unitOfWork.SaveChangesAsync(token);return Map(item);
    }

    private static TransactionTypeSettingDto Map(TransactionTypeSetting item)=>new(item.Code.ToString(),item.Label,item.Effect.ToString());
}
