export function getAuthToken(): string | null {
  return localStorage.getItem("jwt") || sessionStorage.getItem("jwt");
}
export function deleteAuthToken(): void {
  localStorage.setItem("jwt", "");
  sessionStorage.setItem("jwt", "");
}
