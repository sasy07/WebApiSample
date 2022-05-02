﻿namespace WebApiSample.Entities;

public abstract class BaseEntity<TKey> :IEntity
{
    public TKey Id { get; set; }
}

public abstract class BaseEntity : BaseEntity<int>
{
    
}

public interface IEntity
{
    
}