using DataAccessLogic.DatabaseModels;
using DataAccessLogic.HelperServices;
using DataAccessLogic.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLogic.CrudLogic
{
    public class OrderLogic : ICrudLogic<Order>
    {
        private readonly ApplicationContext context;

        public OrderLogic(ApplicationContext context)
        {
            this.context = context;
        }

        public async Task Create(Order model)
        {
            Antiques antique = await context.Antiques
               .Include(antiques => antiques.User)
               .FirstOrDefaultAsync(antiques => antiques.Id == model.AntiquesId);
            User user = await context.Users
                .FirstOrDefaultAsync(user => user.UserName == model.UserName);

            SavedList existingList = await context.SavedLists
                .Include(list => list.Antiques)
                .FirstOrDefaultAsync(list => list.UserId == user.Id);

            Antiques antiquesToRemove = existingList.Antiques
                .Find(antiques => antiques.Id == antique.Id);

            existingList.Antiques.Remove(antiquesToRemove);

            model.Id = Guid.NewGuid().ToString();
            model.User = user;
            model.Antiques = antique;
            antique.Status = AntiquesStatusProvider.GetSoldStatus();

            context.Antiques.Update(antique);
            await context.Orders.AddAsync(model);
            await context.SaveChangesAsync();            
        }

        public async Task Delete(Order model)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Order>> Read(Order model)
        {
            return await context.Orders
                .Include(orders => orders.User)
                .Include(order => order.Antiques)
                .Where(orders => model == null
                || model.User != null && !string.IsNullOrWhiteSpace(model.User.UserName) 
                && orders.UserName == model.User.UserName
                || !string.IsNullOrWhiteSpace(model.Id) && orders.Id == model.Id)
                .ToListAsync();
        }

        public async Task Update(Order model)
        {
            throw new NotImplementedException();
        }
    }
}