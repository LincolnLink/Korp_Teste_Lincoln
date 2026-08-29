import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./layout/main-layout.component/main-layout.component')
        .then(m => m.MainLayoutComponent),
    children: [
      {
        path: 'produtos',
        loadComponent: () =>
          import('./features/produtos/pages/produtos-page.component/produtos-page.component')
            .then(m => m.ProdutosPageComponent)
      },
      {
        path: 'notas',
        loadComponent: () =>
          import('./features/notas-fiscais/pages/notas-page.component/notas-page.component')
            .then(m => m.NotasPageComponent)
      },
      {
        path: '',
        redirectTo: 'produtos',
        pathMatch: 'full'
      }
    ]
  }
];