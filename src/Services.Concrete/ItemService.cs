﻿using Entities;
using Microsoft.EntityFrameworkCore;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services
{
  public class ItemService : IItemService
  {
    private readonly IItemRepository itemRepository;

    public ItemService(IItemRepository itemRepository)
    {
      this.itemRepository = itemRepository;
    }

    public async Task<IEnumerable<ItemDTO>> AllAsync()
    {
      return await itemRepository.All()
        .Select(i => new ItemDTO { Id = i.Id, Text = i.Text, Priority = i.Priority })
        .ToListAsync();
    }

    public async Task<ItemDTO> SaveAsync(ItemDTO itemDTO)
    {
      int maxPriority = await itemRepository.GetMaxItemPriorityAsync();

      Item item = new Item { Text = itemDTO.Text, Priority = maxPriority + 1 };

      await itemRepository.CreateAsync(item);
      await itemRepository.SaveChangesAsync();

      itemDTO.Id = item.Id;
      itemDTO.Priority = item.Priority;

      return itemDTO;
    }

    public async Task UpdateAsync(ItemDTO itemDTO)
    {
      Item item = await itemRepository.GetByIdAsync(itemDTO.Id);

      if (item == null)
        throw new ArgumentException($"Item with id {itemDTO.Id} is not found");

      item.Text = itemDTO.Text;

      if (itemDTO.Priority != item.Priority)
      {
        Item itemToSwapPriority = await itemRepository.GetByPriorityAsync(itemDTO.Priority);

        if (itemToSwapPriority != null)
        {
          itemToSwapPriority.Priority = item.Priority;
          item.Priority = itemDTO.Priority;
        }
      }

      itemRepository.Update(item);

      await itemRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
      Item item = await itemRepository.GetByIdAsync(id);

      if (item == null)
        throw new ArgumentException($"Item with id {id} is not found");

      itemRepository.Delete(item);

      await itemRepository.SaveChangesAsync();
    }
  }
}
