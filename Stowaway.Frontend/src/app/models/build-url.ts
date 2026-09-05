/**
 * Substitutes ":token" placeholders in a route template with real values.
 *
 * Example:
 *  buildUrl('User/:id', { id: 5 })              -> 'User/5'
 *  buildUrl('Cart/clear-cart/:userId', { userId: 12 }) -> 'Cart/clear-cart/12'
 */
export function buildUrl(template: string, params: Record<string, string | number>): string {
  return template.replace(/:([A-Za-z][A-Za-z0-9]*)/g, (match, key: string) => {
    if (!(key in params)) {
      throw new Error(`Missing URL param "${key}" for template "${template}"`);
    }
    return encodeURIComponent(String(params[key]));
  });
}
