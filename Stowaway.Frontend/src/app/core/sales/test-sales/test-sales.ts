import { Component, inject, ViewChild } from '@angular/core';
import { OrderService } from '../../../services/sales/order/order-service';
import { FormArray, FormBuilder, FormControl, FormGroup, ɵInternalFormsSharedModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormField, MatInput, MatLabel } from "@angular/material/input";
import { MatAnchor } from "@angular/material/button";
import { validateHeaderName } from 'http';
import {CreateOrderCommand, SharedOrderCommandContainerType, UpdateOrderCommand} from '../../../services/sales/order/order-service.models'
import { catchError, tap } from 'rxjs';
import { ListSales } from "../list-sales/list-sales";
@Component({
  selector: 'app-test-sales',
  imports: [ɵInternalFormsSharedModule, ReactiveFormsModule, MatFormField, MatInput, MatAnchor, MatLabel, ListSales],
  templateUrl: './test-sales.html',
  styleUrl: './test-sales.css',
})



export class TestSales {
  orderService = inject(OrderService)
  formBuilderService = inject(FormBuilder)
  orderForm = this.formBuilderService.group(
    {
      //inputs go here
      orderId : [0],
      userId : [0, Validators.required],
      
      items : this.formBuilderService.array([]) //[] is the default value in this case
    }
  )
  
  @ViewChild(ListSales)listSales !: ListSales

  constructor()
  {
    this.addOrderItemForm();
    
  }
  
  get itemsArray() : FormArray { return (this.orderForm.get('items') as FormArray);}
  
  addOrderItemForm()
  {
    //console.log(this.itemsArray);
    this.itemsArray.push(this.formBuilderService.group(
      {
        //itemthings go here
        containerTypeId : [1, Validators.required],
        quantity: [1, Validators.required]
      }
    ))
  }
  
  removeOrderItemForm(index : number)
  {
    this.itemsArray.removeAt(index);
  }

  deleteOrder() {
  //throw new Error('Method not implemented.');
  }
  updateOrder() {
    const payload : UpdateOrderCommand = 
    {
      id : this.orderForm.get('orderId')?.value!,
      allContainerTypes : [...this.getItemArray()]
    };
    if(payload.allContainerTypes.length === 0)
    {
      console.error("Items must be added");
      return;
    };
    this.orderService.update(payload).pipe(
      tap((response) => {
        console.log(`updated order -> ${response}`);
        this.listSales.refreshOrders();
      }),
      catchError((err) =>
      {
        console.error("UpdateOrder CatchError  ->", err);
        throw new Error(err);
      })
    ).subscribe();
  }
  createOrder() {
    const payload : CreateOrderCommand = 
    {
      userId : this.orderForm.get('userId')?.value!,
      orderItems : [...this.getItemArray()]
    }
    if(payload.orderItems.length === 0)
    {
      console.error("Items must be added");
      return;
    }
    this.orderService.create(payload).pipe(
      tap((response) => {
        console.log(`Created order -> ${response}`);
        this.listSales.refreshOrders();
      }),
      catchError((err) =>
      {
        console.error("CreateOrder CatchError ->", err);
        throw new Error(err);
      })
    ).subscribe();
    
  }
  getItemArray(): SharedOrderCommandContainerType[] {
    const a = this.itemsArray.getRawValue() as unknown as SharedOrderCommandContainerType[];
    console.log(a);
    return a
  }
}

