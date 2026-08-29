import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NotaFormComponents } from './nota-form.components';

describe('NotaFormComponents', () => {
  let component: NotaFormComponents;
  let fixture: ComponentFixture<NotaFormComponents>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NotaFormComponents],
    }).compileComponents();

    fixture = TestBed.createComponent(NotaFormComponents);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
