import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ProdutoFormComponents } from './produto-form.components';

describe('ProdutoFormComponents', () => {
  let component: ProdutoFormComponents;
  let fixture: ComponentFixture<ProdutoFormComponents>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProdutoFormComponents],
    }).compileComponents();

    fixture = TestBed.createComponent(ProdutoFormComponents);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
