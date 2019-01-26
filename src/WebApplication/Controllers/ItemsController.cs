﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services;
using Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Controllers
{
  [ApiController]
  [Authorize(Roles = "user")]
  [Route("/api/v1/[controller]")]
  public class ItemsController : Controller
  {
    private readonly IItemService itemService;

    public ItemsController(IItemService itemService)
    {
      this.itemService = itemService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ItemDTO>>> AllAsync()
    {
      return new List<ItemDTO>(await itemService.AllAsync());
    }

    [HttpPost]
    public async Task<ActionResult<ItemDTO>> SaveAsync(ItemCreateApiModel item)
    {
      return await itemService.SaveAsync(new ItemDTO { Text = item.Text });
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchAsync(int id, [FromBody]ICollection<PatchDTO> patches)
    {
      IActionResult actionResult = Ok();

      try
      {
        await itemService.UpdatePartiallyAsync(id, patches);
      }
      catch (EntityNotFoundException)
      {
        actionResult = NotFound();
      }
      catch (ArgumentException)
      {
        actionResult = BadRequest();
      }

      return actionResult;
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
      IActionResult actionResult = Ok();

      try
      {
        await itemService.DeleteAsync(id);
      }
      catch (EntityNotFoundException)
      {
        actionResult = NotFound();
      }

      return actionResult;
    }
  }
}
