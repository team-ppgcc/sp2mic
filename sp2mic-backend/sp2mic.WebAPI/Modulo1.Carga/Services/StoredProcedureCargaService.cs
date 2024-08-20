// using Microsoft.EntityFrameworkCore;
// using sp2mic.WebAPI.Context;
// using sp2mic.WebAPI.Domain.Entities;
// using sp2mic.WebAPI.Modulo1.Carga.Services.Interfaces;
// using static sp2mic.WebAPI.CrossCutting.Constantes;
//
// namespace sp2mic.WebAPI.Modulo1.Carga.Services;
//
// public class StoredProcedureCargaService : IStoredProcedureCargaService
// {
//   private readonly DbContextSp2Mic _dbContext;
//
//   public StoredProcedureCargaService (DbContextSp2Mic dbContext)
//     => _dbContext = dbContext ?? throw new ArgumentNullException(nameof (dbContext));
//
//   // public void SaveAll (IEnumerable<StoredProcedure> entityRange)
//   // {
//   //   var listaProcedures = entityRange.Where(NaoExiste).ToList();
//   //   _dbContext.AddRange(listaProcedures);
//   //   _dbContext.SaveChanges();
//   // }
//   //
//   // private bool NaoExiste(StoredProcedure obj)
//   // {
//   //   var commandText
//   //     = $"SELECT * FROM {Schema}.\"StoredProcedure\" WHERE \"No_Schema\" = \'{obj.NoSchema}\' and \"No_StoredProcedure\" = \'{obj.NoStoredProcedure}\'";
//   //   var result = _dbContext.StoredProcedures.FromSqlRaw(commandText).ToHashSet();
//   //   return result.Count == 0;
//   // }
// }
