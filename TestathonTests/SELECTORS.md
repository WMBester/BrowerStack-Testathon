# StackDemo — Element Selectors & Site Findings
> Discovered via Playwright exploration against https://testathon.live
> Last updated: 2026-02-20

---

## Test Accounts

| Username | Password | Purpose |
|---|---|---|
| `demouser` | `testingisfun99` | Standard test user |
| `image_not_loading_user` | `testingisfun99` | Validates broken image handling |
| `existing_orders_user` | `testingisfun99` | Pre-seeded order history |
| `fav_user` | `testingisfun99` | Pre-seeded favourites (5 items) |
| `locked_user` | `testingisfun99` | Locked account scenario |

---

## Base URL

```
https://testathon.live
```

---

## Sign In (`/signin`)

| Element | Selector | Notes |
|---|---|---|
| Username React Select container | `#username` | Click to open dropdown |
| Password React Select container | `#password` | Click to open dropdown |
| Dropdown options | `div[id*="react-select"][id*="option"]` | Options are `<div>` not `[role="option"]` |
| demouser option | `#react-select-2-option-0-0` | |
| image_not_loading_user option | `#react-select-2-option-0-1` | |
| existing_orders_user option | `#react-select-2-option-0-2` | |
| fav_user option | `#react-select-2-option-0-3` | |
| locked_user option | `#react-select-2-option-0-4` | |
| Password option (testingisfun99) | `#react-select-3-option-0-0` | Only one password option |
| Log In button | `#login-btn` | |
| Error message | `h3.api-error` | Shown for locked user, empty credentials, no password |
| React Select hidden input | `#react-select-2-input` | Text typed here is ignored (no custom entry) |

### Sign-In Notes
- Username and password are **React Select dropdowns** — options must be clicked, not typed
- After successful login URL becomes `https://testathon.live/?signin=true`
- Locked user stays on `/signin` with `h3.api-error` visible
- Navigating to `/checkout`, `/favourites`, `/offers`, `/orders` while unauthenticated redirects to `/signin?{page}=true`
- Visiting `/signin` while already logged in redirects to home `/`

---

## Navigation & Header

| Element | Selector | Notes |
|---|---|---|
| StackDemo logo link | `a.Navbar_logo__26S5Y` | `href="/"` — always visible |
| Offers nav link | `a#offers` | `href="/offers"` |
| Orders nav link | `a#orders` | `href="/orders"` |
| Favourites nav link | `a#favourites` | `href="/favourites"` |
| Sign In link (anonymous) | `a#Sign\ In` | Shown when not logged in |
| Logged-in username display | `span.username` | Shows e.g. `demouser` when signed in |
| Nav container (main) | `nav.space-x-4` | `style="display: flex;"` — contains Offers/Orders/Favourites |
| **Logout link** | `a#logout` | Text: "Logout", inside `nav.UserNav_root__343id` |
| User nav container | `nav.UserNav_root__343id` | Contains `span.username` + `a#logout` |

### Navigation Notes
- Nav links use CSS module class `Navbar_link__3Blki`
- Sign-out is `a#logout` — positioned absolutely in the user nav section
- Footer IS present as a `<footer>` HTML tag with `© 2020 BrowserStack. All rights reserved.`

---

## Home & Product Listing (`/`)

### Product Cards

| Element | Selector | Notes |
|---|---|---|
| Product card container | `div.shelf-item` | IDs = product number (1–25) |
| Product title | `p.shelf-item__title` | Plain text |
| Product price (number) | `div.shelf-item__price b` | Just the number e.g. `799` |
| Product price (dollars) | `div.shelf-item__price small` | The `$` sign |
| Product price (cents) | `div.shelf-item__price span` | e.g. `.00` |
| Product image | `div.shelf-item__thumb img` | `alt` = product name |
| Add to Cart button | `div.shelf-item__buy-btn` | A `<div>` not `<button>`. Text: "Add to cart" |
| Favourite (heart) button | `.shelf-stopper button` | `aria-label="delete"`, MUI `IconButton` |
| Favourite button (active/faved) | `.shelf-stopper button.clicked` | Has extra class `clicked` when favourited |
| Products found counter | `small.products-found span` | e.g. `25 Product(s) found.` |

### Category Filters

| Element | Selector | Notes |
|---|---|---|
| Filter section container | `div.filters` | |
| Filter title | `h4.title` | Text: "Vendors:" |
| Apple checkbox | `div.filters input[value="Apple"]` | `type="checkbox"` |
| Samsung checkbox | `div.filters input[value="Samsung"]` | `type="checkbox"` |
| Google checkbox | `div.filters input[value="Google"]` | `type="checkbox"` |
| OnePlus checkbox | `div.filters input[value="OnePlus"]` | `type="checkbox"` |
| Checkbox label/span | `.checkmark` | Visible label text inside `<label>` |

### Sort Control

> ⚠️ **NOT FOUND** — No `<select>` element or sort dropdown was detected on the page.
> The text "Low to High" / "High to Low" / "Price" did not appear in the DOM.
> TC-PL-006 and TC-PL-007 (sort by price) may not be implemented on the live site,
> or the sort control may be hidden behind a specific interaction not yet explored.

### Product Images (image_not_loading_user)
- For `image_not_loading_user`, product images have `naturalWidth === 0` (broken/not loaded)
- For `demouser`, images load normally with `naturalWidth > 0`

---

## Float Cart (sidebar overlay)

| Element | Selector | Notes |
|---|---|---|
| Cart container (closed) | `div.float-cart` | |
| Cart container (open) | `div.float-cart.float-cart--open` | Gets `float-cart--open` class when open |
| Cart quantity badge (header) | `.bag__quantity` | Shows item count e.g. `1`, `2` |
| Cart quantity (in open cart) | `.float-cart .bag__quantity` | |
| Cart close button | `.float-cart__close-btn` | Text: "X" |
| Cart items container | `.float-cart__shelf-container` | |
| Empty cart message | `p.shelf-empty` | Text: "Add some products in the bag :)" |
| Cart item | `.float-cart__shelf-container .shelf-item` | |
| Cart item title | `.float-cart .shelf-item .title` | |
| Cart item description/qty | `.float-cart .shelf-item .desc` | e.g. "Apple \nQuantity: 1" |
| Cart item price | `.float-cart .shelf-item .shelf-item__price p` | e.g. "$ 799.00" |
| Cart item delete | `.shelf-item__del` | |
| Cart item decrease qty | `button.change-product-button:disabled` | Disabled when qty = 1 |
| Cart item increase qty | `button.change-product-button` | Second button |
| Subtotal | `.float-cart .sub-price__val` | e.g. "$ 799.00" |
| Checkout button (in cart) | `.float-cart .buy-btn` | Text: "Checkout" when items present |
| Continue Shopping button | `.float-cart .buy-btn` | Text: "Continue Shopping" when empty |

---

## Checkout (`/checkout`)

| Element | Selector | Notes |
|---|---|---|
| Page heading | `a.checkoutHeader-link` | Text: "StackDemo", `href="/"` |
| Shipping form legend | `legend[data-test="shipping-address-heading"]` | Text: "Shipping Address" |
| First Name input | `#firstNameInput` | `required`, `autocomplete="given-name"` |
| Last Name input | `#lastNameInput` | `required`, `autocomplete="family-name"` |
| Address input | `#addressLine1Input` | `required`, `autocomplete="address-line1"` |
| State/Province input | `#provinceInput` | `required`, `autocomplete="address-level1"`, label: "State/Province" |
| Postal Code input | `#postCodeInput` | `required`, `autocomplete="postal-code"` |
| Submit button | `#checkout-shipping-continue` | `type="submit"`, text: "Submit", class: `button--primary` |
| Order Summary aside | `article.cart[data-test="cart"]` | |
| Order Summary heading | `h3.cart-title` | Text: "Order Summary" |
| Field wrapper (firstName) | `div.dynamic-form-field--firstName` | |
| Field wrapper (lastName) | `div.dynamic-form-field--lastName` | |
| Field wrapper (address) | `div.dynamic-form-field--addressLine1` | |
| Field wrapper (province) | `div.dynamic-form-field--province` | |
| Field wrapper (postCode) | `div.dynamic-form-field--postCode` | |

### Checkout Notes
- All inputs have `required=""` attribute — uses **HTML5 native browser validation** (no JS error divs)
- Inputs have class `form-input optimizedCheckout-form-input`
- Labels use `form-label optimizedCheckout-form-label`
- When required fields are empty and Submit is clicked: form stays on `/checkout`, no redirect — use URL check + submit button still visible to assert validation
- The checkout page renders ONLY when cart has items AND user is authenticated
- Empty cart at `/checkout` shows an empty page shell (React renders but no form)
- Unauthenticated access to `/checkout` redirects to `/signin?checkout=true`

---

## Order Confirmation (`/confirmation`)

| Element | Selector | Notes |
|---|---|---|
| Success message | `legend#confirmation-message` | Text: "Your Order has been successfully placed." |
| Order number | `strong` inside the confirmation div | e.g. `<strong>83</strong>` — numeric 1–100 |
| Download receipt link | `a#downloadpdf` | Text: "Download order receipt", triggers PDF |
| Continue Shopping button | `button.button--tertiary` | Text: "Continue Shopping »", wrapped in `a[href="/"]` |
| Continue Shopping link | `a[href="/"]` wrapping the button | Parent anchor |
| Order Summary section | `article.cart[data-test="cart"]` | Same as checkout |
| Products list | `ul.productList` | |
| Product item | `li.productList-item` | |
| Product title | `h5.product-title` | |
| Product brand/qty | `li.product-option` | Two items: brand and quantity |
| Product price | `div.product-price` | e.g. "$799" |
| Total label | `span.cart-priceItem-label` | Text: "Total (USD)" |
| Total value | `span.cart-priceItem-value span` | e.g. "$799.00" |

### Confirmation Notes
- URL after successful checkout: `/confirmation`
- Heading is a `<legend>` element with `id="confirmation-message"`, not `<h1>`/`<h2>`
- Order number text format: "Your order number is **N**." where N is 1–100

---

## Favourites (`/favourites`)

| Element | Selector | Notes |
|---|---|---|
| Products container | `div.shelf-container` | Same component as homepage |
| Products found count | `small.products-found span` | e.g. "0 Product(s) found." or "5 Product(s) found." |
| Empty state | `small.products-found span` with text "0 Product(s) found." | No dedicated empty-state component |
| Fav product card | `div.shelf-item` | Same structure as homepage |
| Faved heart button | `.shelf-stopper button.clicked` | Has `clicked` class = product is favourited |
| Unfave button | `.shelf-stopper button.clicked` | Click again to remove |
| Add to cart (from favs) | `div.shelf-item__buy-btn` | Same selector as homepage |

### Favourites Notes
- `fav_user` has 5 pre-seeded favourites
- `demouser` starts with 0 favourites
- Unauthenticated access to `/favourites` redirects to `/signin?favourites=true`
- The page uses the exact same `shelf-item` component as the home product listing
- Clicking a faved product's `.shelf-stopper button` removes it from favourites

---

## Orders (`/orders`)

### Empty State (demouser)

| Element | Selector | Notes |
|---|---|---|
| Orders listing container | `div.orders-listing` | |
| Empty state heading | `h2` | Text: "No orders found" |

### Existing Orders (existing_orders_user)

| Element | Selector | Notes |
|---|---|---|
| Order card | `div.order` | IDs = "1", "2", etc. |
| Order info box | `div.a-box.a-color-offset-background.order-info` | |
| Order placed label | `span.a-color-secondary.label` | Text: "Order placed" |
| Order placed value | `span.a-color-secondary.value` | e.g. "1 November, 2020" |
| Total label | second `span.label` | Text: "Total" |
| Total value | second `span.value` | e.g. "$125" |
| Ship to label | third `span.label` | Text: "Ship to" |
| Ship to value | third `span.value` | e.g. "existing_orders_user" |
| Delivery status box | `div.shipment.shipment-is-delivered` | |
| Delivered date | `span.a-size-medium.a-color-base.a-text-bold` | e.g. "Delivered 2 November, 2020" |
| Order item image | `img.item-image` | |
| Order item title | `div.a-row:has(strong:text("Title:"))` | Contains `<strong>Title:</strong>` |
| Order item description | `div.a-row:has(strong:text("Description:"))` | |
| Order item price | `span.a-size-small.a-color-price` | e.g. "$10.00" |

### Orders Notes
- Unauthenticated access to `/orders` redirects to `/signin?orders=true`
- `demouser` shows empty state with "No orders found"
- `existing_orders_user` has at least 1 pre-seeded order

---

## Offers (`/offers`)

| Element | Selector | Notes |
|---|---|---|
| Geolocation granted — promo message | `div.p-6.text-2xl` | Text: "We've promotional offers in your location." |
| Offer card | `div.offer` | IDs: "iphone", "dekstop", "shipping" |
| Offer image | `div.offer img` | `style="height: 150px;"` |
| Offer title | `div.offer-title` | e.g. "30% off on iPhones" |
| Geolocation denied message | `div.pt-6.text-2xl.font-bold` | Text: "Please enable Geolocation in your browser." |
| No offers for location | *(not captured)* | Expected text: "Sorry we do not have any promotional offers in your city." |
| No geo support (old browser) | *(not captured)* | Expected text: "Geolocation is not available in your browser." |

### Offers Notes
- Unauthenticated access to `/offers` redirects to `/signin?offers=true`
- Geolocation context setup (Playwright): `{ geolocation: { latitude: 37.7749, longitude: -122.4194 }, permissions: ['geolocation'] }`
- Geolocation denied = default browser context (no permissions granted)
- 3 offer cards shown when geolocation is granted: iPhone, OnePlus, Free Shipping

---

## Helper Patterns (C# Playwright)

### Sign In Helper
```csharp
private async Task SelectDropdownOption(string containerId, string optionText)
{
    await Page.Locator($"#{containerId}").ClickAsync();
    await Page.GetByRole(AriaRole.Option, new() { Name = optionText, Exact = true }).ClickAsync();
}

private async Task SignIn(string username)
{
    await SelectDropdownOption("username", username);
    await SelectDropdownOption("password", "testingisfun99");
    await Page.Locator("#login-btn").ClickAsync();
}
```
> Note: C# Playwright's `GetByRole(AriaRole.Option)` works against React Select even though
> the DOM shows `<div>` elements (Playwright resolves ARIA roles implicitly).

### Add Product to Cart
```csharp
await Page.WaitForSelectorAsync(".shelf-item__buy-btn");
await Page.Locator(".shelf-item__buy-btn").First.ClickAsync();
```

### Fill Checkout Form
```csharp
await Page.Locator("#firstNameInput").FillAsync("John");
await Page.Locator("#lastNameInput").FillAsync("Doe");
await Page.Locator("#addressLine1Input").FillAsync("123 Main Street");
await Page.Locator("#provinceInput").FillAsync("California");
await Page.Locator("#postCodeInput").FillAsync("90001");
await Page.Locator("#checkout-shipping-continue").ClickAsync();
```

### Grant Geolocation (C# Playwright)
```csharp
await Context.GrantPermissionsAsync(new[] { "geolocation" });
await Context.SetGeolocationAsync(new Geolocation { Latitude = 37.7749f, Longitude = -122.4194f });
```

---

## Footer

| Element | Selector | Notes |
|---|---|---|
| Footer tag | `footer` | Present on all pages |
| Footer text | `footer span` | Text: "© 2020 BrowserStack. All rights reserved." |

---

## Outstanding Unknowns

| Item | Status | Notes |
|---|---|---|
| Sort control (TC-PL-006, TC-PL-007) | ❌ Not found | No `<select>` or sort UI exists in the DOM — feature not implemented on live site |
