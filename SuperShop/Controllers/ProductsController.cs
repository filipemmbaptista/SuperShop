using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuperShop.Data;
using SuperShop.Data.Entities;
using SuperShop.Helpers;
using SuperShop.Models;

namespace SuperShop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly IUserHelper _userHelper;
        //Substituido pelo BlobHelper
        //private readonly IImageHelper _imageHelper;
        private readonly IConverterHelper _converterHelper;
        //Substitui o ImageHelper
        private readonly IBlobHelper _blobHelper;

        public ProductsController(IProductRepository productRepository, IUserHelper userHelper, IConverterHelper converterHelper, IBlobHelper blobHelper)
        {
            _productRepository = productRepository;
            _userHelper = userHelper;
            //_imageHelper = imageHelper;
            _converterHelper = converterHelper;
            _blobHelper = blobHelper;
        }

        // GET: Products
        public IActionResult Index()
        {
            return View(_productRepository.GetAll().OrderBy(p => p.Name));
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("ProductNotFound");
            }

            var product = await _productRepository.GetByIdAsync(id.Value);
            if (product == null)
            {
                return new NotFoundViewResult("ProductNotFound");
            }

            return View(product);
        }

        // GET: Products/Create
        [Authorize(Roles ="Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Used for ImageHelper
                //var path = string.Empty; 

                // Used for BlobHelper
                Guid imageId = Guid.NewGuid();

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    // Used for ImageHelper
                    //path = await _imageHelper.UploadImageAsync(model.ImageFile, "products");

                    // Used for BlobHelper
                    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "products");
                }

                //For BlobHelper -> change converterHelper to receive a Guid instead of String for the 2nd parameter
                var product = _converterHelper.ToProduct(model, imageId, true);

                product.User = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
                await _productRepository.CreateAsync(product);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Products/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("ProductNotFound");
            }

            var product = await _productRepository.GetByIdAsync(id.Value);
            if (product == null)
            {
                return new NotFoundViewResult("ProductNotFound");
            }

            var model = _converterHelper.ToProductViewModel(product);

            return View(model);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    /* Used for ImageHelper
                    * var path = model.ImageUrl; */

                    // Used for BlobHelper
                    Guid imageId = model.ImageId;

                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        /* Used for ImageHelper
                        * path = await _imageHelper.UploadImageAsync(model.ImageFile, "products"); */

                        // Used for BlobHelper
                        imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "products");
                    }

                    //For BlobHelper -> change converterHelper to receive a Guid instead of String for the 2nd parameter
                    var product = _converterHelper.ToProduct(model, imageId, false);

                    product.User = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
                    await _productRepository.UpdateAsync(product);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _productRepository.ExistAsync(model.Id))
                    {
                        return new NotFoundViewResult("ProductNotFound");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Products/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("ProductNotFound");
            }

            var product = await _productRepository.GetByIdAsync(id.Value);
            if (product == null)
            {
                return new NotFoundViewResult("ProductNotFound");
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            try
            {
                await _productRepository.DeleteAsync(product);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("DELETE"))
                {
                    ViewBag.ErrorTitle = $"{product.Name} should be in use!";
                    ViewBag.ErrorMessage = $"{product.Name} cannot be deleted while they are under orders.</br></br>" +
                        $"Try delete the order first, and then delete the product."; 
                }
                return View("Error");
            }

        }

        public IActionResult ProductNotFound()
        {
            return View();
        }
    }
}
