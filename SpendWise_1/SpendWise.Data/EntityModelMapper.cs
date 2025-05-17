using System;
using System.Collections.Generic;
using System.Linq;

namespace SpendWise.Data
{
    public static class EntityModelMapper
    {
        // Convert from entity to model
        public static Models.FinancialTransactionModel ToModel(this FinancialTransaction entity)
        {
            if (entity == null) return null;

            return new Models.FinancialTransactionModel(
                entity.Description,
                entity.Amount,
                entity.IsExpense,
                entity.TransactionCategory?.Name ?? "Unknown",
                entity.Date
            );
        }

        // Convert from model to entity
        public static FinancialTransaction ToEntity(this Models.FinancialTransactionModel model)
        {
            if (model == null) return null;

            // Note: You might need to set CategoryId or look up the category
            var entity = new FinancialTransaction(
                model.Description,
                model.Amount,
                model.IsExpense,
                model.Category,
                model.Date
            );

            return entity;
        }

        // Convert TransactionCategory <-> TransactionCategoryModel
        public static Models.TransactionCategoryModel ToModel(this TransactionCategory entity)
        {
            if (entity == null) return null;

            return new Models.TransactionCategoryModel(
                entity.Name,
                entity.Description ?? ""
            );
        }

        // Convert Event <-> EventModel
        public static Models.UserEventModel ToModel(this Event entity)
        {
            if (entity == null) return null;

            return new Models.UserEventModel(
                entity.UserId,
                entity.Description
            );
        }

        // For collections
        public static List<Models.FinancialTransactionModel> ToModelList(this IEnumerable<FinancialTransaction> entities)
        {
            return entities?.Select(e => e.ToModel()).ToList() ?? new List<Models.FinancialTransactionModel>();
        }

        public static List<FinancialTransaction> ToEntityList(this IEnumerable<Models.FinancialTransactionModel> models)
        {
            return models?.Select(m => m.ToEntity()).ToList() ?? new List<FinancialTransaction>();
        }
    }
}